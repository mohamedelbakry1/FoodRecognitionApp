using FoodRecognitionApp.Persistence.Identity.Entities;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Persistence.Identity.Services
{
    internal class IdentityService(UserManager<UserAccount> _userManager) : IIdentityService
    {
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User.NotFound", $"User with email {email} is not found");
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterRequest register, CancellationToken ct = default)
        {
            var user = new UserAccount()
            {
                Email = register.Email,
                DisplayName = register.DisplayName,
                UserName = register.DisplayName,
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description, ErrorType.Failure)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }

            return new IdentityUserResult(user.Id, user.Email, user.DisplayName, user.UserName);
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User.NotFound", $"User with email {email} is not found");

            return new IdentityUserResult(user.Id, user.Email!, user.DisplayName, user.UserName!);
        }

        public async Task<Result<IdentityUserResult>> FindUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("User.NotFound", $"User with ID {userId} is not found");

            return new IdentityUserResult(user.Id, user.Email!, user.DisplayName, user.UserName!);
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User.NotFound", $"User with email {email} is not found");

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result> ResetPasswordAsync(string email, string newPassword, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("User.NotFound", $"User with email {email} is not found");
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.Errors.Select(e => new Error(e.Code, e.Description, ErrorType.Failure)).ToList();
                return Result.Fail(errors);
            }
            var addResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e => new Error(e.Code, e.Description, ErrorType.Failure)).ToList();
                return Result.Fail(errors);
            }
            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<IdentityUserResult>>> GetAllUsersAsync(CancellationToken ct = default)
        {
            var users = await _userManager.Users
                .Select(u => new IdentityUserResult(u.Id, u.Email!, u.DisplayName, u.UserName!))
                .ToListAsync(ct);

            return users;
        }
    }
}
