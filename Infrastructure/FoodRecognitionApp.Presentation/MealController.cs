using FoodRecognitionApp.Domain.Exceptions.UnAuthorized;
using FoodRecognitionApp.Services.Abstraction;
using FoodRecognitionApp.Shared.Dtos.Meal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FoodRecognitionApp.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet("foods")]
        public async Task<IActionResult> GetAllFoods()
        {
            var result = await _serviceManager.MealService.GetAllFoodsAsync();
            return Ok(result);
        }

        [HttpPost("log")]
        public async Task<IActionResult> CreateMeal(CreateMealRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _serviceManager.MealService.AddMealAsync(userId, request);
            return Ok(result);
        }

        [HttpPut("{mealId}/type")]
        public async Task<IActionResult> UpdateMealType(int mealId, UpdateMealTypeRequest request)
        {
            var userId = GetCurrentUserId();
            await _serviceManager.MealService.UpdateMealTypeAsync(userId, mealId, request);
            return Ok("Meal Type Updated Successfully");
        }
        [HttpPut("{mealId}/items")]
        public async Task<IActionResult> UpdateMealItems(int mealId, UpdateMealItemsRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _serviceManager.MealService.UpdateMealItemsAsync(userId, mealId, request);
            return Ok(result);
        }

        [HttpDelete("{mealId}")]
        public async Task<IActionResult> DeleteMeal(int mealId)
        {
            var userId = GetCurrentUserId();
            await _serviceManager.MealService.DeleteMealAsync(userId, mealId);
            return Ok("Meal Deleted Successfully");
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayMeals()
        {
            var userId = GetCurrentUserId();
            var result = await _serviceManager.MealService.GetTodayMealsAsync(userId);
            return Ok(result);
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummary()
        {
            var userId = GetCurrentUserId();
            var result = await _serviceManager.MealService.GetDailySummaryAsync(userId);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMealHistory([FromQuery] DateTime date)
        {
            var userId = GetCurrentUserId();
            var result = await _serviceManager.MealService.GetMealHistoryAsync(userId, date);
            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null) throw new UnAuthorizedException("You are not Authorized");
            return int.Parse(userIdClaim.Value);
        }
    }
}
