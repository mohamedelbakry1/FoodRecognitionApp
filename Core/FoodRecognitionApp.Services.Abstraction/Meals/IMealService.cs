using FoodRecognitionApp.Shared.Dtos.Meal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Meals
{
    public interface IMealService
    {
        Task<MealResponse> AddMealAsync(int userId, CreateMealRequest request);
        Task UpdateMealTypeAsync(int userId, int mealId, UpdateMealTypeRequest request);
        Task<MealResponse> UpdateMealItemsAsync(int userId, int mealId, UpdateMealItemsRequest request);
        Task DeleteMealAsync(int userId, int mealId);
        Task<TodayMealsResponse> GetTodayMealsAsync(int userId);
        Task<TodayMealsResponse> GetMealHistoryAsync(int userId, DateTime date);
        Task<IEnumerable<FoodItemResponse>> GetAllFoodsAsync();
        Task<DailySummaryResponse> GetDailySummaryAsync(int userId);
    }
}
