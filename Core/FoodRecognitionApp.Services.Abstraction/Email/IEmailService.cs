using FoodRecognitionApp.Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Email
{
    public interface IEmailService
    {
        Task<Result> SendOtpAsync(string email, string otp, CancellationToken ct = default);
        Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
        Task<Result> SendWelcomeEmailAsync(string email, string displayName, CancellationToken ct = default);
    }
}
