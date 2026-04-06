using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Shared.Dtos.Email;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FoodRecognitionApp.Services.Email
{
    public class EmailService(IOptions<EmailSettings> _options) : IEmailService
    {
        public async Task SendOtpAsync(string email, string otp)
        {
            var settings = _options.Value;

            var smtpClient = new SmtpClient(settings.Host)
            {
                Port = settings.Port,
                Credentials = new NetworkCredential(settings.Email, settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(settings.Email, settings.DisplayName),
                Subject = "FoodRecognitionApp - Reset Password OTP",
                Body = $@"
                    <h2>Password Reset Request</h2>
                    <p>Your OTP code is:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{otp}</h1>
                    <p>This OTP is valid for <strong>10 minutes</strong>.</p>
                    <p>If you didn't request this, please ignore this email.</p>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
