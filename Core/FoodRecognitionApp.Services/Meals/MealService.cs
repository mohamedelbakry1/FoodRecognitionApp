using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
using FoodRecognitionApp.Services.Abstraction.Meals;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Dtos.Meal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Meals
{
    public class MealService(IUnitOfWork _unitOfWork) : IMealService
    {
        public async Task<MealResponse> AddMealAsync(int userId, CreateMealRequest request)
        {
            var foods = await ValidateAndGetFoodsAsync(request.Items);

            var (mealItems, totalCalories) = BuildMealItems(request.Items, foods);

            var meal = new Meal 
            {
                UserId = userId,
                MealType = request.MealType,
                TotalCalories = totalCalories,
                MealItems = mealItems
            };

            await _unitOfWork.GetRepository<int, Meal>().AddAsync(meal);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count <= 0) throw new CreateMealBadRequestException();

            return BuildMealResponse(meal.Id, request.MealType, meal.MealTime, totalCalories, request.Items, foods);
        }

        public async Task UpdateMealTypeAsync(int userId, int mealId, UpdateMealTypeRequest request)
        {
            var spec = new MealByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec);
            if (meal is null) throw new MealNotFoundException(mealId);

            meal.MealType = request.MealType;

            _unitOfWork.GetRepository<int, Meal>().Update(meal);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count <= 0) throw new UpdateMealBadRequestException();
        }

        public async Task<MealResponse> UpdateMealItemsAsync(int userId, int mealId, UpdateMealItemsRequest request)
        {
            var spec = new MealWithItemsByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec);
            if (meal is null) throw new MealNotFoundException(mealId);

            var foods = await ValidateAndGetFoodsAsync(request.Items);

            meal.MealItems.Clear();

            var (mealItems, totalCalories) = BuildMealItems(request.Items, foods);

            foreach (var item in mealItems)
                meal.MealItems.Add(item);

            meal.TotalCalories = totalCalories;

            _unitOfWork.GetRepository<int, Meal>().Update(meal);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count <= 0) throw new UpdateMealBadRequestException();

            return BuildMealResponse(meal.Id, meal.MealType, meal.MealTime, totalCalories, request.Items, foods);
        }

        public async Task DeleteMealAsync(int userId, int mealId)
        {
            var spec = new MealByIdAndUserSpecification(userId, mealId);
            var meal = await _unitOfWork.GetRepository<int, Meal>().GetById(spec);

            if(meal is null) throw new MealNotFoundException(mealId);
            
            _unitOfWork.GetRepository<int, Meal>().Delete(meal);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count <= 0) throw new DeleteMealBadRequestException();
        }

        public async Task<IEnumerable<FoodItemResponse>> GetAllFoodsAsync()
        {
            var spec = new AllFoodsSpecification();
            var Foods = await _unitOfWork.GetRepository<int, Food>().GetAllAsync(spec);

            return Foods.Select(F => new FoodItemResponse
            {
                FoodId = F.Id,
                Name = F.Name,
                Calories = F.Calories,
                Carbs = F.Carbs,
                Protein = F.Protein,
                Fats = F.Fats,
                CategoryName = F.CategoryFood.CategoryName
            });
        }

        public async Task<DailySummaryResponse> GetDailySummaryAsync(int userId)
        {
            var profileSpec = new ProfileByUserIdSpecification(userId);
            var profile = await _unitOfWork.GetRepository<int,UserProfile>().GetById(profileSpec);

            decimal caloriesTarget = profile?.Daily_Calories ?? 0;

            var meals = await GetMealsByDateAsync(userId, DateTime.UtcNow.Date);

            decimal caloriesConsumed = meals.TotalCalories;
            decimal caloreisRemaining = caloriesTarget - caloriesConsumed;
            var goalPercentage = caloriesTarget > 0 ? Math.Round((caloriesConsumed / caloriesTarget) * 100,1) : 0;

            return new DailySummaryResponse
            {
                Date = DateTime.UtcNow.Date.ToString(),
                CaloriesTarget = caloriesTarget,
                CaloriesConsumed = caloriesConsumed,
                CaloriesRemaining = caloreisRemaining,
                GoalPercentage = goalPercentage,
                TotalCarbs = meals.TotalCarbs,
                TotalProtein = meals.TotalProteins,
                TotalFats = meals.TotalFats
            };
        }

        public async Task<TodayMealsResponse> GetTodayMealsAsync(int userId)
        {
            return await GetMealsByDateAsync(userId, DateTime.UtcNow.Date);
        }

        public async Task<TodayMealsResponse> GetMealHistoryAsync(int userId, DateTime date)
        {
            return await GetMealsByDateAsync(userId, date);
        }


        #region Helper Methods

        private async Task<List<Food>> ValidateAndGetFoodsAsync(List<MealItemRequest> items)
        {
            var duplicateFoodId = items
                .GroupBy(i => i.FoodId)
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateFoodId is not null)
                throw new DuplicateFoodItemBadRequestException(duplicateFoodId.Key);

            var foodIds = items.Select(i => i.FoodId).ToList();
            var spec = new FoodsByIdSpecification(foodIds);
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(spec)).ToList();

            foreach (var item in items)
            {
                if (!foods.Any(f => f.Id == item.FoodId))
                    throw new FoodNotFoundException(item.FoodId.ToString());
            }

            return foods;
        }

        private (List<MealItem> mealItems, decimal totalCalories) BuildMealItems(List<MealItemRequest> items, List<Food> foods)
        {
            var mealItems = new List<MealItem>();
            decimal totalCalories = 0;

            foreach (var item in items)
            {
                var food = foods.FirstOrDefault(f => f.Id == item.FoodId);
                var itemCalories = (food!.Calories / 100m) * item.Quantity;
                totalCalories += itemCalories;

                mealItems.Add(new MealItem
                {
                    FoodId = item.FoodId,
                    Quantity = item.Quantity,
                    Item_Calories = itemCalories
                });
            }
            return (mealItems, totalCalories);
        }

        private MealResponse BuildMealResponse(int mealId, MealType mealType, DateTime mealTime, decimal totalCalories, List<MealItemRequest> items, List<Food> foods)
        {
            return new MealResponse
            {
                MealId = mealId,
                MealType = mealType.ToString(),
                MealTime = mealTime,
                TotalCalories = totalCalories,
                Items = items.Select(item =>
                {
                    var food = foods.FirstOrDefault(f => f.Id == item.FoodId);
                    return new MealItemResponse
                    {
                        FoodName = food!.Name,
                        Quantity = item.Quantity,
                        Item_Calories = (food!.Calories / 100m) * item.Quantity,
                        Carbs = (food!.Carbs / 100m) * item.Quantity,
                        Protein = (food!.Protein / 100m) * item.Quantity,
                        Fats = (food!.Fats / 100m) * item.Quantity
                    };
                }).ToList()
            };
        }

        private async Task<TodayMealsResponse> GetMealsByDateAsync(int userId, DateTime date)
        {
            var mealSpec = new MealsByDateSpecification(userId, date);
            var meals = (await _unitOfWork.GetRepository<int, Meal>()
                        .GetAllAsync(mealSpec)).ToList();

            if (!meals.Any())
            {
                return new TodayMealsResponse
                {
                    Date = date.Date.ToString(),
                    TotalCalories = 0,
                    TotalCarbs = 0,
                    TotalProteins = 0,
                    TotalFats = 0,
                    Meals = new List<MealResponse>()
                };
            }

            var foodIds = meals.SelectMany(m => m.MealItems).Select(mi => mi.FoodId).Distinct().ToList();

            var foodSpec = new FoodsByIdSpecification(foodIds);
            var foods = (await _unitOfWork.GetRepository<int, Food>()
                .GetAllAsync(foodSpec, changeTracker: false)).ToList();

            decimal totalCalories = 0, totalCarbs = 0, totalProtein = 0, totalFats = 0;
            var mealResponses = meals.Select(meal =>
            {
                var items = meal.MealItems.Select(mi =>
                {
                    var food = foods.FirstOrDefault(f => f.Id == mi.FoodId);
                    var carbs = (food!.Carbs / 100m) * mi.Quantity;
                    var protein = (food!.Protein / 100m) * mi.Quantity;
                    var fats = (food!.Fats / 100m) * mi.Quantity;

                    totalCarbs += carbs;
                    totalProtein += protein;
                    totalFats += fats;

                    return new MealItemResponse
                    {
                        FoodName = food.Name,
                        Quantity = mi.Quantity,
                        Item_Calories = mi.Item_Calories,
                        Carbs = carbs,
                        Protein = protein,
                        Fats = fats
                    };
                }).ToList();

                totalCalories += meal.TotalCalories;

                return new MealResponse
                {
                    MealId = meal.Id,
                    MealType = meal.MealType.ToString(),
                    MealTime = meal.MealTime,
                    TotalCalories = meal.TotalCalories,
                    Items = items
                };
            }).ToList();

            return new TodayMealsResponse
            {
                Date = date.Date.ToString(),
                TotalCalories = totalCalories,
                TotalCarbs = totalCarbs,
                TotalProteins = totalProtein,
                TotalFats = totalFats,
                Meals = mealResponses
            };
        }
        #endregion
    }
}
