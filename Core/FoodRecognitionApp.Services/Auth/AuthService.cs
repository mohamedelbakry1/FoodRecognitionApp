using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Auth
{
    internal class AuthService(
        IIdentityService _identityService,
        ITokenService _tokenService,
        IEmailService _emailService,
        ILogger<AuthService> _logger,
        IMemoryCache _memoryCache
    ) : IAuthService
    {
        private static readonly Random _random = new();
        public async Task<Result<UserResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Error.InvalidCredentials("Auth.InvalidCredentials", "The email or password is incorrect.");

            var userResult = await _identityService.FindUserByEmailAsync(request.Email, ct);
            if (userResult.IsFailure)
                return Error.InvalidCredentials("Auth.InvalidCredentials", "The email or password is incorrect.");


            var passwordValid = await _identityService.CheckPasswordAsync(request.Email, request.Password, ct);
            if (passwordValid.IsFailure || !passwordValid.Value)
                return Error.InvalidCredentials("Auth.InvalidCredentials", "The email or password is incorrect.");

            var user = userResult.Value;
 
            var rolesResult = await _identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.IsSuccess ? rolesResult.Value : Array.Empty<string>();
            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.UserName, roles);
            return new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };
        }
        public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (request is null)
                return Error.Validation("Auth.InvalidRequest", "Registration data is required.");

            var createResult = await _identityService.CreateUserAsync(request, ct);
            if (createResult.IsFailure)
                return createResult.Errors.ToList();
            var user = createResult.Value;

            var rolesResult = await _identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.IsSuccess ? rolesResult.Value : Array.Empty<string>();
            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.UserName, roles);

            var emailResult = await _emailService.SendWelcomeEmailAsync(user.Email, user.DisplayName, ct);
            if (emailResult.IsFailure)
            {
                var errorDetails = string.Join(" | ", emailResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                _logger.LogWarning(
                    $"User registered successfully (Id: {user.Id}, Email: {user.Email}), but welcome email failed to send. Reason: {errorDetails}");
            }

            return new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };
        }
        public async Task<Result<UserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken ct = default)
        {
            var userResult = await _identityService.FindUserByIdAsync(userId, ct);
            if (userResult.IsFailure)
                return Error.NotFound("Auth.UserNotFound", "User with the specified ID does not exist.");

            var user = userResult.Value;

            var rolesResult = await _identityService.GetUserRolesAsync(user.Email, ct);
            var roles = rolesResult.IsSuccess ? rolesResult.Value : Array.Empty<string>();

            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.UserName, roles);
            return new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };
        }
        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email))
                return Error.Validation("Auth.InvalidEmail", "Email is required.");

            var userResult = await _identityService.FindUserByEmailAsync(request.Email, ct);
            if (userResult.IsFailure)
                return Error.NotFound("Auth.UserNotFound", "User with the specified email does not exist.");

            var otp = _random.Next(100000, 999999).ToString();
            var cacheKey = GetOtpCacheKey(request.Email);
            _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendOtpAsync(request.Email, otp, ct);
            return Result.Ok();
        }
        public Result VerifyOtp(VerifyOtpRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                return Error.Validation("Auth.InvalidOtp", "The provided OTP is invalid or has expired.");

            return ValidateOtp(request.Email, request.Otp);
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            if (request is null)
                return Error.Validation("Auth.InvalidRequest", "Request data is required.");

            var otpValidation = ValidateOtp(request.Email, request.Otp);
            if (otpValidation.IsFailure)
                return otpValidation;

            var resetResult = await _identityService.ResetPasswordAsync(request.Email, request.Password, ct);
            if (resetResult.IsFailure)
                return resetResult;

            _memoryCache.Remove(GetOtpCacheKey(request.Email));
            return Result.Ok();
        }
        private Result ValidateOtp(string email, string otp)
        {
            var cacheKey = GetOtpCacheKey(email);
            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != otp)
                return Error.Validation("Auth.InvalidOtp", "The provided OTP is invalid or has expired.");
            return Result.Ok();
        }
        private static string GetOtpCacheKey(string email) => $"Otp {email}";
    }
}
