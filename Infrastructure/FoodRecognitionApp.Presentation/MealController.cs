using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Shared.Dtos.Meal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    [Authorize]
    public class MealController(IMealService _mealService) : ApiBaseController
    {
        [HttpGet("foods")]
        [ProducesResponseType(typeof(IReadOnlyList<FoodItemResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FoodItemResponse>>> GetAllFoods(CancellationToken ct)
        {
            var result = await _mealService.GetAllFoodsAsync(ct);
            return ToActionResult(result);
        }
        [HttpPost("log")]
        [ProducesResponseType(typeof(MealResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MealResponse>> CreateMeal([FromBody] CreateMealRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.AddMealAsync(userId, request, ct);
            return ToActionResult(result);
        }
        [HttpPut("{mealId:int}/type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMealType(int mealId, [FromBody] UpdateMealTypeRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.UpdateMealTypeAsync(userId, mealId, request, ct);
            return ToActionResult(result);
        }
        [HttpPut("{mealId:int}/items")]
        [ProducesResponseType(typeof(MealResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MealResponse>> UpdateMealItems(int mealId, [FromBody] UpdateMealItemsRequest request, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.UpdateMealItemsAsync(userId, mealId, request, ct);
            return ToActionResult(result);
        }
        [HttpDelete("{mealId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMeal(int mealId, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.DeleteMealAsync(userId, mealId, ct);
            return ToActionResult(result);
        }
        [HttpGet("today")]
        [ProducesResponseType(typeof(TodayMealsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<TodayMealsResponse>> GetTodayMeals(CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.GetTodayMealsAsync(userId, ct);
            return ToActionResult(result);
        }
        [HttpGet("daily-summary")]
        [ProducesResponseType(typeof(DailySummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DailySummaryResponse>> GetDailySummary(CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.GetDailySummaryAsync(userId, ct);
            return ToActionResult(result);
        }
        [HttpGet("history")]
        [ProducesResponseType(typeof(TodayMealsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<TodayMealsResponse>> GetMealHistory([FromQuery] DateTime date, CancellationToken ct)
        {
            var userId = GetUserIdFromToken();
            var result = await _mealService.GetMealHistoryAsync(userId, date, ct);
            return ToActionResult(result);
        }
    }
}
