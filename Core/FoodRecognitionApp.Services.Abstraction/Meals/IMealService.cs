using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Meal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Meals
{
    public interface IMealService
    {
        Task<Result<MealResponse>> AddMealAsync(Guid userId, CreateMealRequest request, CancellationToken ct = default);
        Task<Result> UpdateMealTypeAsync(Guid userId, int mealId, UpdateMealTypeRequest request, CancellationToken ct = default);
        Task<Result<MealResponse>> UpdateMealItemsAsync(Guid userId, int mealId, UpdateMealItemsRequest request, CancellationToken ct = default);
        Task<Result> DeleteMealAsync(Guid userId, int mealId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<FoodItemResponse>>> GetAllFoodsAsync(CancellationToken ct = default);
        Task<Result<DailySummaryResponse>> GetDailySummaryAsync(Guid userId, CancellationToken ct = default);
        Task<Result<TodayMealsResponse>> GetTodayMealsAsync(Guid userId, CancellationToken ct = default);
        Task<Result<TodayMealsResponse>> GetMealHistoryAsync(Guid userId, DateTime date, CancellationToken ct = default);
    }
}
