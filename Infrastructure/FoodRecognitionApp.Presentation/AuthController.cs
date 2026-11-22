using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    public class AuthController(IAuthService _authService) : ApiBaseController
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> Login(LoginRequest request, CancellationToken ct = default)
        {
            var result = await _authService.LoginAsync(request, ct);
            return ToActionResult(result);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> Register(RegisterRequest request, CancellationToken ct = default)
        {
            var result = await _authService.RegisterAsync(request, ct);
            return ToActionResult(result);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct = default)
        {
            var result = await _authService.ForgotPasswordAsync(request, ct);
            return ToActionResult(result);
        }

        [HttpPost("verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult VerifyOtp(VerifyOtpRequest request)
        {
            var result = _authService.VerifyOtp(request);
            return ToActionResult(result);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct = default)
        {
            var result = await _authService.ResetPasswordAsync(request, ct);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken ct = default)
        {
            var userId = GetUserIdFromToken();
            var result = await _authService.GetCurrentUserAsync(userId, ct);
            return ToActionResult(result);
        }
    }
}
