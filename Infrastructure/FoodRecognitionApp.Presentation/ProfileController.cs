using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Shared.Dtos.Profile;
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
    public class ProfileController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("setup")]
        public async Task<IActionResult> CreateProfile(CreateProfileRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) throw new UnAuthorizedException("You are Not Authorized ");
            var userId = int.Parse(userIdString);

            var result = await _serviceManager.ProfileService.CreateProfileAsync(userId, request);
            return Ok(result);
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(CreateProfileRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) throw new UnAuthorizedException("You are Not Authorized ");
            var userId = int.Parse(userIdString);

            await _serviceManager.ProfileService.UpdateProfileAsync(userId, request);
            return Ok("Profile Updated Successfully");
        }
    }
}
