using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodRecognitionController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("recognize")]
        public async Task<IActionResult> RecognizeFood(FoodRecognitionRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null) throw new UnAuthorizedException("You are not Authorized");

            var userId = int.Parse(userIdClaim.Value);

            var result = await _serviceManager.FoodRecognitionService.RecognizeFoodAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentRecogntions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null) throw new UnAuthorizedException("You are not Authorized");
            var userId = int.Parse(userIdClaim.Value);

            var result = await _serviceManager.FoodRecognitionService.GetRecentRecognitionsAsync(userId);
            return Ok(result);
        }
    }
}
