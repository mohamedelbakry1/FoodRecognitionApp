using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Shared.Dtos.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FoodRecognitionApp.Services.Email
{
    public class EmailService(IOptions<EmailSettings> _options) : IEmailService
    {
        public async Task SendOtpAsync(string email, string otp)
        {
            var settings = _options.Value;

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(settings.DisplayName, settings.Email));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "FoodRecognitionApp - Password Reset OTP";
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Password Reset Request</h2>
                    <p>Your OTP code is:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{otp}</h1>
                    <p>This OTP is valid for <strong>10 minutes</strong>.</p>
                    <p>If you didn't request this, please ignore this email.</p>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings.Email, settings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

        }
    }
}
