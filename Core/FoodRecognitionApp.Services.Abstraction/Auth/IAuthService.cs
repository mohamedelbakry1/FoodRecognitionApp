using FoodRecognitionApp.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<UserResponse?> LoginAsync(LoginRequest request);
        Task<UserResponse?> RegisterAsync(RegisterRequest request);
        Task<UserResponse?> GetCurrentUserAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        bool VerifyOtp(VerifyOtpRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
