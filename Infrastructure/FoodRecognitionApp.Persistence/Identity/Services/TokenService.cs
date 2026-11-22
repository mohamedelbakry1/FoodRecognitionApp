using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Shared.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Persistence.Identity.Services
{
    internal class TokenService(IOptions<JwtSettings> _jwtSettings, ILogger<TokenService> _logger) : ITokenService
    {
        public string GenerateToken(string userId, string email, string userName, IReadOnlyList<string> roles)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName cannot be null or empty.", nameof(userName));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null && roles.Count > 0)
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var secKey = _jwtSettings.Value.SecurityKey;
            if (string.IsNullOrWhiteSpace(secKey))
                throw new InvalidOperationException("Secret Key is not found");

            if (secKey.Length < 32)
                throw new InvalidOperationException("Secret Key is too short");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresInDays = _jwtSettings.Value.DurationInDays;
            if (expiresInDays <= 0)
                throw new InvalidOperationException("Token duration must be greater than zero.");

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(expiresInDays),
                signingCredentials: signingCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("Generated JWT for userId {UserId} (expires in {Days} days)", userId, expiresInDays);

            return tokenString;
        }
    }
}
