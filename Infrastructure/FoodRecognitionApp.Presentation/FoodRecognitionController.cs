using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Presentation
{
    [Authorize]
    public class FoodRecognitionController(IFoodRecognitionService _foodRecognitionService) : ApiBaseController
    {
        [HttpPost("recognize")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(FoodRecognitionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FoodRecognitionResponse>> RecognizeFood([FromForm] FoodRecognitionRequest request, CancellationToken ct = default)
        {
            var userId = GetUserIdFromToken();

            var result = await _foodRecognitionService.RecognizeFoodAsync(userId, request, ct);
            return ToActionResult(result);
        }

        [HttpGet("recent")]
        [ProducesResponseType(typeof(ICollection<RecentRecognitionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<RecentRecognitionResponse>>> GetRecentRecogntions(CancellationToken ct = default)
        {
            var userId = GetUserIdFromToken();

            var result = await _foodRecognitionService.GetRecentRecognitionsAsync(userId, ct);
            return ToActionResult(result);
        }


    }
}
