using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _serviceManager.AuthService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _serviceManager.AuthService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _serviceManager.AuthService.ForgotPasswordAsync(request);
            return Ok();
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
        {
            var result = _serviceManager.AuthService.VerifyOtp(request);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _serviceManager.AuthService.ResetPasswordAsync(request);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.AuthService.GetCurrentUserAsync(email!.Value);
            return Ok(result);
        }
    }
}
