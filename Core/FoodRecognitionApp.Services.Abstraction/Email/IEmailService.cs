using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Email
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string otp);
    }
}
