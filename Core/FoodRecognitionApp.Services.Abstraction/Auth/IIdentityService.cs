using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Auth
{
    public interface IIdentityService
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default);
        Task<Result<IdentityUserResult>> FindUserByIdAsync(Guid userId, CancellationToken ct = default);
        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);
        Task<Result<IdentityUserResult>> CreateUserAsync(RegisterRequest register, CancellationToken ct = default);
        Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken ct = default);
        Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<Result> ResetPasswordAsync(string email, string newPassword, CancellationToken ct = default);
        Task<Result<IReadOnlyList<IdentityUserResult>>> GetAllUsersAsync(CancellationToken ct = default);
    }
}
