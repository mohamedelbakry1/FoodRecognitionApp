using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<Result<UserResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<Result<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<Result<UserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken ct = default);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
        Result VerifyOtp(VerifyOtpRequest request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
    }
}
