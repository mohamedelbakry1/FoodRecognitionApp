using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Shared;
using FoodRecognitionApp.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FoodRecognitionApp.Services.Auth
{
    public class AuthService(
        UserManager<UserAccount> _userManager, 
        IOptions<JwtOptions> options,
        IEmailService _emailService,
        IMemoryCache _memoryCache
        ) : IAuthService
    {
        private static readonly Random _random = new Random();
        public async Task<UserResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user is null) throw new UserNotFoundException(request.Email);

            var flag = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!flag) throw new UnAuthorizedException("You are not Authorized !!");

            return new UserResponse()
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
        {
            var user = new UserAccount()
            {
                UserName = request.UserName,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded) throw new RegisterationBadRequestException(result.Errors.Select(E => E.Description).ToList());

            return new UserResponse()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<UserResponse?> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);

            return new UserResponse()
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Token = await GenerateTokenAsync(user),
            };
        }

        private async Task<string> GenerateTokenAsync(UserAccount user)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtOptions = options.Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                expires: DateTime.Now.AddDays(jwtOptions.DurationInDays),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) throw new UserNotFoundException(request.Email);

            var otp = _random.Next(100000, 999999).ToString();

            var cacheKey = GetOtpCacheKey(request.Email);
            _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(10));

            await _emailService.SendOtpAsync(request.Email, otp);
        }

        public bool VerifyOtp(VerifyOtpRequest request)
        {
            ValidOtp(request.Email, request.Otp);
            return true;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            ValidOtp(request.Email, request.Otp);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) throw new UserNotFoundException(request.Email);

            var removedPassword = await _userManager.RemovePasswordAsync(user);
            if(!removedPassword.Succeeded) throw new RemovePasswordBadRequestException();

            var addPassword = await _userManager.AddPasswordAsync(user, request.Password);
            if(!addPassword.Succeeded) throw new ResetPasswordBadRequestException(addPassword.Errors.Select(E => E.Description).ToList());

            _memoryCache.Remove(GetOtpCacheKey(request.Email));
        }


        private void ValidOtp(string email, string otp)
        {
            var cacheKey = GetOtpCacheKey(email);

            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != otp) 
                throw new InvalidOtpBadRequestException();
        }

        private static string GetOtpCacheKey(string email) => $"Otp {email}";
    }
}
