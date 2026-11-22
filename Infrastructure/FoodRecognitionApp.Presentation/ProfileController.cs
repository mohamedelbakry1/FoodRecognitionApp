using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Shared.Dtos.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Presentation
{
    [Authorize]
    public class ProfileController(IProfileService _profileService) : ApiBaseController
    {
        [HttpPost("setup")]
        [ProducesResponseType(typeof(CreateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateProfileResponse>> CreateProfile(CreateProfileRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _profileService.CreateProfileAsync(userId, request, ct);
            return ToActionResult(result);
        }
        [HttpPut("update")]
        [ProducesResponseType(typeof(CreateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateProfileResponse>> UpdateProfile(CreateProfileRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _profileService.UpdateProfileAsync(userId, request, ct);
            return ToActionResult(result);
        }
    }
}
