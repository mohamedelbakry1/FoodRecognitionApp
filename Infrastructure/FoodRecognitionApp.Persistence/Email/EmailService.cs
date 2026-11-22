using FoodRecognitionApp.Services.Abstraction.Email;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Email;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Text;
using MailKit;

namespace FoodRecognitionApp.Persistence.Email
{
    internal class EmailService(
        IOptions<EmailSettings> _options,
        ILogger<EmailService> _logger) : IEmailService
    {
        public async Task<Result> SendOtpAsync(string email, string otp, CancellationToken ct = default)
        {
            var subject = "FoodRecognitionApp - Password Reset OTP";
            var body = BuildOtpTemplate(otp);
            return await SendEmailAsync(email, subject, body, ct);
        }
        public async Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(to))
                return Error.Validation("Email.InvalidRecipient", "Recipient email cannot be empty.");
            var settings = _options.Value;
            if (string.IsNullOrWhiteSpace(settings.Host) || string.IsNullOrWhiteSpace(settings.Email))
            {
                _logger.LogError("Email settings are not properly configured in appsettings.");
                return Error.Failure("Email.ConfigError", "Email service is not configured properly.");
            }
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.DisplayName, settings.Email));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };
                using var smtp = new SmtpClient();
                _logger.LogInformation("Connecting to SMTP server {Host}:{Port}...", settings.Host, settings.Port);
                await smtp.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto, ct);
                await smtp.AuthenticateAsync(settings.Email, settings.Password, ct);
                _logger.LogInformation("Sending email to {Recipient}...", to);
                await smtp.SendAsync(message, cancellationToken: ct);
                await smtp.DisconnectAsync(true, ct);
                _logger.LogInformation("Email sent successfully to {Recipient}.", to);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}.", to);
                return Error.Failure("Email.SendFailed", $"Failed to send email: {ex.Message}");
            }
        }

        public async Task<Result> SendWelcomeEmailAsync(string email, string displayName, CancellationToken ct = default)
        {
            var subject = "🥗 Welcome to Food Recognition AI - Let's Start Your Journey!";
            var body = BuildWelcomeTemplate(displayName);

            return await SendEmailAsync(email, subject, body, ct);
        }


        private static string BuildOtpTemplate(string otp)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset OTP</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f7f6; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
    <table border='0' cellpadding='0' cellspacing='0' width='100%' style='table-layout: fixed; background-color: #f4f7f6; padding: 40px 0;'>
        <tr>
            <td align='center'>
                
                <!-- Main Container -->
                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 520px; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.05); border: 1px solid #e5e7eb;'>
                    
                    <!-- Header with Gradient & App Icon -->
                    <tr>
                        <td align='center' style='padding: 35px 20px 20px 20px; background: linear-gradient(135deg, #10B981 0%, #059669 100%);'>
                            <div style='background-color: #ffffff; width: 64px; height: 64px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; box-shadow: 0 4px 10px rgba(0,0,0,0.1); margin-bottom: 12px;'>
                                <span style='font-size: 32px;'>🥗</span>
                            </div>
                            <h2 style='margin: 0; color: #ffffff; font-size: 22px; font-weight: 700; letter-spacing: -0.5px;'>
                                Food Recognition AI
                            </h2>
                            <p style='margin: 4px 0 0 0; color: #d1fae5; font-size: 13px; font-weight: 500;'>
                                Smart Nutrition & Meal Tracking
                            </p>
                        </td>
                    </tr>
                    <!-- Body Content -->
                    <tr>
                        <td style='padding: 35px 35px 25px 35px; color: #1f2937;'>
                            
                            <h3 style='margin: 0 0 12px 0; font-size: 19px; font-weight: 700; color: #111827; text-align: center;'>
                                Password Reset Request
                            </h3>
                            
                            <p style='margin: 0 0 25px 0; font-size: 14px; line-height: 1.6; color: #4b5563; text-align: center;'>
                                We received a request to reset your password. Use the verification code below to complete the process.
                            </p>
                            <!-- OTP Box -->
                            <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom: 25px;'>
                                <tr>
                                    <td align='center'>
                                        <div style='background: #ecfdf5; border: 2px dashed #10B981; border-radius: 12px; padding: 18px 25px; display: inline-block;'>
                                            <span style='font-family: ""SFMono-Regular"", Consolas, Monaco, monospace; font-size: 34px; font-weight: 800; letter-spacing: 8px; color: #065f46;'>
                                                {otp}
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <!-- Expiration Notice -->
                            <div style='text-align: center; margin-bottom: 25px;'>
                                <span style='display: inline-block; background-color: #fef3c7; color: #92400e; padding: 6px 14px; border-radius: 20px; font-size: 12px; font-weight: 600;'>
                                    ⏱ Valid for 10 minutes only
                                </span>
                            </div>
                            <!-- Security Warning Card -->
                            <div style='background-color: #f9fafb; border-left: 4px solid #ef4444; border-radius: 4px; padding: 12px 16px; margin-bottom: 20px;'>
                                <p style='margin: 0; font-size: 12px; line-height: 1.5; color: #6b7280;'>
                                    <strong style='color: #ef4444;'>Security Tip:</strong> Never share this code with anyone. Our team will never ask for your password or verification code.
                                </p>
                            </div>
                            <p style='margin: 0; font-size: 13px; line-height: 1.5; color: #9ca3af; text-align: center;'>
                                If you did not request a password reset, you can safely ignore this email.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='padding: 20px 30px; background-color: #f9fafb; border-top: 1px solid #f3f4f6; text-align: center;'>
                            <p style='margin: 0 0 6px 0; font-size: 12px; color: #9ca3af;'>
                                &copy; 2026 Food Recognition App. All rights reserved.
                            </p>
                            <p style='margin: 0; font-size: 11px; color: #d1d5db;'>
                                This is an automated security email, please do not reply.
                            </p>
                        </td>
                    </tr>
                </table>
                <!-- End Main Container -->
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private static string BuildWelcomeTemplate(string displayName)
        {
            var name = string.IsNullOrWhiteSpace(displayName) ? "there" : displayName;
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to Food Recognition AI</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f7f6; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
    <table border='0' cellpadding='0' cellspacing='0' width='100%' style='table-layout: fixed; background-color: #f4f7f6; padding: 40px 0;'>
        <tr>
            <td align='center'>
                
                <!-- Main Container -->
                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 560px; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.05); border: 1px solid #e5e7eb;'>
                    
                    <!-- Header -->
                    <tr>
                        <td align='center' style='padding: 40px 20px 30px 20px; background: linear-gradient(135deg, #10B981 0%, #059669 100%);'>
                            <div style='background-color: #ffffff; width: 70px; height: 70px; border-radius: 50%; display: inline-flex; align-items: center; justify-content: center; box-shadow: 0 4px 12px rgba(0,0,0,0.15); margin-bottom: 15px;'>
                                <span style='font-size: 36px;'>🥗</span>
                            </div>
                            <h1 style='margin: 0; color: #ffffff; font-size: 24px; font-weight: 800; letter-spacing: -0.5px;'>
                                Welcome to Food Recognition AI!
                            </h1>
                            <p style='margin: 6px 0 0 0; color: #d1fae5; font-size: 14px; font-weight: 500;'>
                                Your AI-Powered Nutrition & Wellness Companion
                            </p>
                        </td>
                    </tr>
                    <!-- Body Content -->
                    <tr>
                        <td style='padding: 35px 35px 25px 35px; color: #1f2937;'>
                            
                            <h2 style='margin: 0 0 15px 0; font-size: 20px; font-weight: 700; color: #111827;'>
                                Hi {name}, 👋
                            </h2>
                            
                            <p style='margin: 0 0 25px 0; font-size: 15px; line-height: 1.6; color: #4b5563;'>
                                We’re thrilled to have you on board! Tracking your nutrition and achieving your healthy lifestyle goals has never been easier. Just snap a photo of your meal, and our AI does the rest.
                            </p>
                            <!-- Features List -->
                            <div style='background-color: #f9fafb; border-radius: 12px; padding: 20px; margin-bottom: 25px; border: 1px solid #f3f4f6;'>
                                <h3 style='margin: 0 0 15px 0; font-size: 15px; font-weight: 700; color: #065f46;'>
                                    ✨ What you can do with our app:
                                </h3>
                                
                                <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                    <tr>
                                        <td style='padding: 8px 0; font-size: 14px; color: #374151;'>
                                            📸 <strong>Instant Food Recognition:</strong> Snap a photo to automatically identify your meal.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-size: 14px; color: #374151;'>
                                            ⚡ <strong>Accurate Macro & Calories:</strong> Get instant breakdowns of Calories, Protein, Carbs, and Fats.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-size: 14px; color: #374151;'>
                                            📊 <strong>Daily Progress & Insights:</strong> Track your nutritional trends and build healthy habits.
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <!-- Call to Action Button -->
                            <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom: 25px;'>
                                <tr>
                                    <td align='center'>
                                        <a href='#' style='background: linear-gradient(135deg, #10B981 0%, #059669 100%); color: #ffffff; text-decoration: none; padding: 14px 32px; border-radius: 30px; font-size: 15px; font-weight: 700; display: inline-block; box-shadow: 0 4px 15px rgba(16, 185, 129, 0.4);'>
                                            Start Tracking Your First Meal 🚀
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            <p style='margin: 0; font-size: 14px; line-height: 1.5; color: #6b7280; text-align: center;'>
                                Need any help getting started? Simply reply to this email or reach out to our support team.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='padding: 20px 30px; background-color: #f9fafb; border-top: 1px solid #f3f4f6; text-align: center;'>
                            <p style='margin: 0 0 6px 0; font-size: 12px; color: #9ca3af;'>
                                &copy; 2026 Food Recognition App. All rights reserved.
                            </p>
                            <p style='margin: 0; font-size: 11px; color: #d1d5db;'>
                                You received this email because you recently signed up for a Food Recognition account.
                            </p>
                        </td>
                    </tr>
                </table>
                <!-- End Main Container -->
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
