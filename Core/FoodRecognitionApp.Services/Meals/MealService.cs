using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Meal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Meals
{
    internal class MealService(IUnitOfWork _unitOfWork, ILogger<MealService> _logger) : IMealService
    {
        public async Task<Result<MealResponse>> AddMealAsync(Guid userId, CreateMealRequest request, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            var foodsResult = await ValidateAndGetFoodsAsync(request.Items, ct);
            if (foodsResult.IsFailure)
                return foodsResult.Errors.ToList();

            var foods = foodsResult.Value;
            var (mealItems, totalCalories) = BuildMealItems(request.Items, foods);

            var meal = new Meal
            {
                UserId = userId,
                MealType = request.MealType,
                MealTime = DateTime.UtcNow,
                TotalCalories = totalCalories,
                MealItems = mealItems
            };

            _unitOfWork.GetRepository<int, Meal>().Add(meal);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            if (count <= 0)
            {
                _logger.LogError("Failed to add meal for user {UserId}", userId);
                return Error.Failure("Meal.CreateFailed", "Failed to save the meal. Please try again.");
            }

            _logger.LogInformation("Meal {MealId} successfully created for user {UserId}", meal.Id, userId);
            return BuildMealResponse(meal.Id, meal.MealType, meal.MealTime, totalCalories, request.Items, foods);
        }
        public async Task<Result> UpdateMealTypeAsync(Guid userId, int mealId, UpdateMealTypeRequest request, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            var spec = new MealByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec, changeTracker: true, ct: ct);
            if (meal is null)
                return Error.NotFound("Meal.NotFound", $"Meal with ID {mealId} was not found.");

            meal.MealType = request.MealType;
            _unitOfWork.GetRepository<int, Meal>().Update(meal);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            if (count <= 0)
            {
                _logger.LogError("Failed to update meal type for meal {MealId}", mealId);
                return Error.Failure("Meal.UpdateFailed", "Failed to update meal type.");
            }

            return Result.Ok();
        }
        public async Task<Result<MealResponse>> UpdateMealItemsAsync(Guid userId, int mealId, UpdateMealItemsRequest request, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            var spec = new MealWithItemsByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec, changeTracker: true, ct: ct);
            if (meal is null)
                return Error.NotFound("Meal.NotFound", $"Meal with ID {mealId} was not found.");

            var foodsResult = await ValidateAndGetFoodsAsync(request.Items, ct);
            if (foodsResult.IsFailure)
                return foodsResult.Errors.ToList();

            var foods = foodsResult.Value;
            var (newItems, totalCalories) = BuildMealItems(request.Items, foods);

            meal.MealItems.Clear();
            foreach (var item in newItems)
            {
                meal.MealItems.Add(item);
            }
            meal.TotalCalories = totalCalories;

            _unitOfWork.GetRepository<int, Meal>().Update(meal);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            if (count <= 0)
            {
                _logger.LogError("Failed to update items for meal {MealId}", mealId);
                return Error.Failure("Meal.UpdateFailed", "Failed to update meal items.");
            }

            return BuildMealResponse(meal.Id, meal.MealType, meal.MealTime, totalCalories, request.Items, foods);
        }
        public async Task<Result> DeleteMealAsync(Guid userId, int mealId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            var spec = new MealByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec, changeTracker: true, ct: ct);
            if (meal is null)
                return Error.NotFound("Meal.NotFound", $"Meal with ID {mealId} was not found.");

            _unitOfWork.GetRepository<int, Meal>().Delete(meal);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            if (count <= 0)
            {
                _logger.LogError("Failed to delete meal {MealId}", mealId);
                return Error.Failure("Meal.DeleteFailed", "Failed to delete the meal.");
            }

            return Result.Ok();
        }
        public async Task<Result<IReadOnlyList<FoodItemResponse>>> GetAllFoodsAsync(CancellationToken ct = default)
        {
            var spec = new AllFoodsSpecification();
            var foods = await _unitOfWork.GetRepository<int, Food>().GetAllAsync(spec, changeTracker: false, ct: ct);

            var response = foods.Select(f => new FoodItemResponse
            {
                FoodId = f.Id,
                Name = f.Name,
                Calories = f.Calories,
                Carbs = f.Carbs,
                Protein = f.Protein,
                Fats = f.Fats,
                CategoryName = f.CategoryFood?.CategoryName ?? string.Empty
            }).ToList();

            return response;
        }
        public async Task<Result<DailySummaryResponse>> GetDailySummaryAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            var profileSpec = new ProfileByUserIdSpecification(userId);
            var profile = await _unitOfWork.GetRepository<int, UserProfile>().GetById(profileSpec, changeTracker: false, ct: ct);

            decimal caloriesTarget = profile?.Daily_Calories ?? 0;

            var mealsResult = await GetMealsByDateAsync(userId, DateTime.UtcNow.Date, ct);
            if (mealsResult.IsFailure)
                return mealsResult.Errors.ToList();

            var meals = mealsResult.Value;
            decimal caloriesConsumed = meals.TotalCalories;
            decimal caloriesRemaining = caloriesTarget - caloriesConsumed;
            var goalPercentage = caloriesTarget > 0 ? Math.Round((caloriesConsumed / caloriesTarget) * 100, 1) : 0;

            return new DailySummaryResponse
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                CaloriesTarget = caloriesTarget,
                CaloriesConsumed = caloriesConsumed,
                CaloriesRemaining = caloriesRemaining,
                GoalPercentage = goalPercentage,
                TotalCarbs = meals.TotalCarbs,
                TotalProtein = meals.TotalProteins,
                TotalFats = meals.TotalFats
            };
        }
        public async Task<Result<TodayMealsResponse>> GetTodayMealsAsync(Guid userId, CancellationToken ct = default)
        {
            return await GetMealsByDateAsync(userId, DateTime.UtcNow.Date, ct);
        }
        public async Task<Result<TodayMealsResponse>> GetMealHistoryAsync(Guid userId, DateTime date, CancellationToken ct = default)
        {
            return await GetMealsByDateAsync(userId, date.Date, ct);
        }
        #region Helper Methods
        private async Task<Result<List<Food>>> ValidateAndGetFoodsAsync(List<MealItemRequest> items, CancellationToken ct)
        {
            if (items is null || items.Count == 0)
                return Error.Validation("Meal.EmptyItems", "Meal must contain at least one item.");

            var duplicateFoodId = items.GroupBy(i => i.FoodId).FirstOrDefault(g => g.Count() > 1);
            if (duplicateFoodId is not null)
                return Error.Validation("Meal.DuplicateFood", $"Duplicate food item detected with ID: {duplicateFoodId.Key}");

            var foodIds = items.Select(i => i.FoodId).Distinct().ToList();
            var spec = new FoodsByIdSpecification(foodIds);
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(spec, changeTracker: false, ct: ct)).ToList();

            var missingIds = foodIds.Except(foods.Select(f => f.Id)).ToList();

            if (missingIds.Count > 0)
                return Error.NotFound("Food.NotFound", $"The following food IDs were not found: {string.Join(", ", missingIds)}");

            return foods;
        }
        private static (List<MealItem> mealItems, decimal totalCalories) BuildMealItems(List<MealItemRequest> items, List<Food> foods)
        {
            var mealItems = new List<MealItem>();
            decimal totalCalories = 0;
            var foodsMap = foods.ToDictionary(f => f.Id);

            foreach (var item in items)
            {
                if (foodsMap.TryGetValue(item.FoodId, out var food))
                {
                    var itemCalories = Math.Round((food.Calories / 100m) * item.Quantity, 2);
                    totalCalories += itemCalories;

                    mealItems.Add(new MealItem
                    {
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Item_Calories = itemCalories
                    });
                }
            }

            return (mealItems, Math.Round(totalCalories, 2));
        }
        private static MealResponse BuildMealResponse(int mealId, MealType mealType, DateTime mealTime, decimal totalCalories, List<MealItemRequest> items, List<Food> foods)
        {
            var foodsMap = foods.ToDictionary(f => f.Id);

            return new MealResponse
            {
                MealId = mealId,
                MealType = mealType.ToString(),
                MealTime = mealTime,
                TotalCalories = totalCalories,
                Items = items.Select(item =>
                {
                    foodsMap.TryGetValue(item.FoodId, out var food);
                    var factor = item.Quantity / 100m;
                    return new MealItemResponse
                    {
                        FoodName = food?.Name ?? string.Empty,
                        Quantity = item.Quantity,
                        Item_Calories = Math.Round((food?.Calories ?? 0) * factor, 2),
                        Carbs = Math.Round((food?.Carbs ?? 0) * factor, 2),
                        Protein = Math.Round((food?.Protein ?? 0) * factor, 2),
                        Fats = Math.Round((food?.Fats ?? 0) * factor, 2)
                    };
                }).ToList()
            };
        }
        private async Task<Result<TodayMealsResponse>> GetMealsByDateAsync(Guid userId, DateTime date, CancellationToken ct)
        {
            var mealSpec = new MealsByDateSpecification(userId, date);
            var meals = (await _unitOfWork.GetRepository<int, Meal>().GetAllAsync(mealSpec, changeTracker: false, ct: ct)).ToList();

            if (meals.Count == 0)
            {
                return new TodayMealsResponse
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalCalories = 0,
                    TotalCarbs = 0,
                    TotalProteins = 0,
                    TotalFats = 0,
                    Meals = []
                };
            }
            var foodIds = meals.SelectMany(m => m.MealItems).Select(mi => mi.FoodId).Distinct().ToList();

            var foodSpec = new FoodsByIdSpecification(foodIds);
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodSpec, changeTracker: false, ct: ct)).ToList();

            var foodsMap = foods.ToDictionary(f => f.Id);

            decimal totalCarbs = 0, totalProtein = 0, totalFats = 0;
            var mealResponses = new List<MealResponse>();
            foreach (var meal in meals)
            {
                var items = new List<MealItemResponse>();
                foreach (var mi in meal.MealItems)
                {
                    foodsMap.TryGetValue(mi.FoodId, out var food);
                    var factor = mi.Quantity / 100m;
                    var carbs = Math.Round((food?.Carbs ?? 0) * factor, 2);
                    var protein = Math.Round((food?.Protein ?? 0) * factor, 2);
                    var fats = Math.Round((food?.Fats ?? 0) * factor, 2);
                    totalCarbs += carbs;
                    totalProtein += protein;
                    totalFats += fats;
                    items.Add(new MealItemResponse
                    {
                        FoodName = food?.Name ?? string.Empty,
                        Quantity = mi.Quantity,
                        Item_Calories = mi.Item_Calories,
                        Carbs = carbs,
                        Protein = protein,
                        Fats = fats
                    });
                }

                mealResponses.Add(new MealResponse
                {
                    MealId = meal.Id,
                    MealType = meal.MealType.ToString(),
                    MealTime = meal.MealTime,
                    TotalCalories = meal.TotalCalories,
                    Items = items
                });
            }

            var totalCalories = mealResponses.Sum(m => m.TotalCalories);

            return new TodayMealsResponse
            {
                Date = date.ToString("yyyy-MM-dd"),
                TotalCalories = Math.Round(totalCalories, 2),
                TotalCarbs = Math.Round(totalCarbs, 2),
                TotalProteins = Math.Round(totalProtein, 2),
                TotalFats = Math.Round(totalFats, 2),
                Meals = mealResponses
            };
        }
        #endregion
    }
}
