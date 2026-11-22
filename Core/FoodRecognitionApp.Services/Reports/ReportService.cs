using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Services.Abstraction.Auth;
using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Services.Abstraction.Reports.Models;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodRecognitionApp.Services.Reports
{
    public class ReportService(
        IUnitOfWork _unitOfWork,
        IIdentityService _identityService,
        IPdfReportGenerator _pdfReportGenerator,
        ILogger<ReportService> _logger
    ) : IReportService
    {
        #region 1. System Overview Report
        public async Task<Result<byte[]>> GenerateSystemOverviewReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating System Overview Report");

            var usersResult = await _identityService.GetAllUsersAsync(ct);
            var users = usersResult.IsSuccess ? usersResult.Value : Array.Empty<FoodRecognitionApp.Shared.Dtos.Auth.IdentityUserResult>();

            var categoriesSpec = new AllCategoriesWithFoodsSpecification();
            var categories = (await _unitOfWork.GetRepository<int, Category>().GetAllAsync(categoriesSpec, changeTracker: false, ct: ct)).ToList();

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false, ct: ct)).ToList();

            var imagesSpec = new AllImagesWithRecognitionsSpecification();
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false, ct: ct)).ToList();

            var allRecognitions = images.SelectMany(i => i.RecognitionResults ?? Enumerable.Empty<RecognitionResult>()).ToList();

            var data = new SystemOverviewData
            {
                TotalUsers = users.Count,
                TotalImages = images.Count,
                TotalRecognitions = allRecognitions.Count,
                TotalFoods = foods.Count,
                TotalCategories = categories.Count,
                AvgConfidence = allRecognitions.Count > 0 ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                FoodsByCategory = categories.Select(c => new CategoryFoodCount
                {
                    CategoryName = c.CategoryName,
                    FoodCount = c.Foods?.Count ?? 0
                }).Where(x => x.FoodCount > 0).ToList(),
                TopRecognizedFoods = foods
                    .Where(f => f.RecognitionResults != null && f.RecognitionResults.Count > 0)
                    .Select(f => new FoodRecognitionCount
                    {
                        FoodName = f.Name,
                        TimesRecognized = f.RecognitionResults.Count
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .Take(10)
                    .ToList()
            };

            return _pdfReportGenerator.GenerateSystemOverviewReport(data);
        }
        #endregion

        #region 2. Food Recognition Analysis Report
        public async Task<Result<byte[]>> GenerateFoodRecognitionReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating Food Recognition Report");

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false, ct: ct)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Count > 0).ToList();
            var allRecognitions = recognizedFoods.SelectMany(f => f.RecognitionResults).ToList();

            var data = new FoodRecognitionReportData
            {
                TotalRecognitions = allRecognitions.Count,
                AvgConfidence = allRecognitions.Count > 0 ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                MinConfidence = allRecognitions.Count > 0 ? allRecognitions.Min(r => r.Confidence_Score) : 0,
                MaxConfidence = allRecognitions.Count > 0 ? allRecognitions.Max(r => r.Confidence_Score) : 0,
                TopFoodsByConfidence = recognizedFoods
                    .Select(f => new FoodConfidenceItem
                    {
                        FoodName = f.Name,
                        CategoryName = f.CategoryFood?.CategoryName ?? "Unknown",
                        AvgConfidence = f.RecognitionResults.Average(r => r.Confidence_Score),
                        TimesRecognized = f.RecognitionResults.Count
                    })
                    .OrderByDescending(x => x.AvgConfidence)
                    .Take(10)
                    .ToList(),
                TopFoodsByCount = recognizedFoods
                    .Select(f => new FoodRecognitionCount
                    {
                        FoodName = f.Name,
                        TimesRecognized = f.RecognitionResults.Count
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .Take(10)
                    .ToList(),
                AllFoodDetails = recognizedFoods
                    .Select(f => new FoodRecognitionDetail
                    {
                        FoodName = f.Name,
                        CategoryName = f.CategoryFood?.CategoryName ?? "Unknown",
                        TimesRecognized = f.RecognitionResults.Count,
                        AvgConfidence = f.RecognitionResults.Average(r => r.Confidence_Score),
                        MinConfidence = f.RecognitionResults.Min(r => r.Confidence_Score),
                        MaxConfidence = f.RecognitionResults.Max(r => r.Confidence_Score)
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .ToList()
            };

            return _pdfReportGenerator.GenerateFoodRecognitionReport(data);
        }
        #endregion

        #region 3. User Demographics Report
        public async Task<Result<byte[]>> GenerateUserDemographicsReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating User Demographics Report");

            var profilesSpec = new AllUserProfilesSpecification();
            var profiles = (await _unitOfWork.GetRepository<int, UserProfile>().GetAllAsync(profilesSpec, changeTracker: false, ct: ct)).ToList();

            var usersResult = await _identityService.GetAllUsersAsync(ct);
            var usersMap = usersResult.IsSuccess
                ? usersResult.Value.ToDictionary(u => u.Id)
                : new Dictionary<Guid, FoodRecognitionApp.Shared.Dtos.Auth.IdentityUserResult>();

            var data = new UserDemographicsData
            {
                TotalUsers = profiles.Count,
                MaleCount = profiles.Count(p => p.Gender == Gender.Male),
                FemaleCount = profiles.Count(p => p.Gender == Gender.Female),
                AvgAge = profiles.Count > 0 ? profiles.Average(p => p.Age) : 0,
                AvgWeight = profiles.Count > 0 ? (double)profiles.Average(p => p.Weight) : 0,
                AvgHeight = profiles.Count > 0 ? (double)profiles.Average(p => p.Height) : 0,
                MaintainWeightCount = profiles.Count(p => p.GoalType == GoalType.MaintainWeight),
                LoseWeightCount = profiles.Count(p => p.GoalType == GoalType.LoseWeight),
                GainWeightCount = profiles.Count(p => p.GoalType == GoalType.GainWeight),
                SedentaryCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.Sedentary),
                LightlyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.LightlyActive),
                ModeratelyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.ModeratelyActive),
                HighlyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.HighlyActive),

                UserDetails = profiles.Select(p =>
                {
                    usersMap.TryGetValue(p.AccountId, out var account);
                    return new UserProfileDetail
                    {
                        UserName = account?.UserName ?? "N/A",
                        Email = account?.Email ?? "N/A",
                        Age = p.Age,
                        Weight = p.Weight,
                        Height = p.Height,
                        Gender = p.Gender.ToString(),
                        GoalType = p.GoalType.ToString(),
                        ActivityLevel = p.ActivityLevel.ToString(),
                        DailyCalories = p.Daily_Calories
                    };
                }).ToList()
            };

            return _pdfReportGenerator.GenerateUserDemographicsReport(data);
        }
        #endregion

        #region 4. User Recognition History Report
        public async Task<Result<byte[]>> GenerateUserRecognitionHistoryReportAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");

            _logger.LogInformation("Generating User Recognition History Report for UserId: {UserId}", userId);

            var userResult = await _identityService.FindUserByIdAsync(userId, ct);
            if (userResult.IsFailure)
                return Error.NotFound("User.NotFound", $"User with ID {userId} was not found.");

            var user = userResult.Value;

            var imagesSpec = new UserImagesWithRecognitionsSpecification(userId);
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false, ct: ct)).ToList();

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false, ct: ct)).ToList();
            var foodDict = foods.ToDictionary(f => f.Id);

            var allRecognitions = images.SelectMany(i => (i.RecognitionResults ?? Enumerable.Empty<RecognitionResult>())
                .Select(r => new { Image = i, Recognition = r })).ToList();

            var data = new UserRecognitionHistoryData
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalImages = images.Count,
                TotalRecognitions = allRecognitions.Count,
                AvgConfidence = allRecognitions.Count > 0 ? allRecognitions.Average(r => r.Recognition.Confidence_Score) : 0,
                Recognitions = allRecognitions.Select(r =>
                {
                    foodDict.TryGetValue(r.Recognition.FoodId, out var food);
                    return new RecognitionHistoryItem
                    {
                        UploadTime = r.Image.UploadTime,
                        FoodName = food?.Name ?? "Unknown",
                        CategoryName = food?.CategoryFood?.CategoryName ?? "Unknown",
                        Confidence = r.Recognition.Confidence_Score
                    };
                }).OrderByDescending(x => x.UploadTime).ToList(),
                MonthlyTrend = allRecognitions
                    .GroupBy(r => r.Image.UploadTime.ToString("yyyy-MM"))
                    .Select(g => new MonthlyRecognitionCount
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList()
            };

            return _pdfReportGenerator.GenerateUserRecognitionHistoryReport(data);
        }
        #endregion

        #region 5. Nutritional Insights Report
        public async Task<Result<byte[]>> GenerateNutritionalInsightsReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating Nutritional Insights Report");

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false, ct: ct)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Count > 0).ToList();

            var data = new NutritionalInsightsData
            {
                TotalRecognitions = recognizedFoods.Sum(f => f.RecognitionResults.Count),
                AvgCalories = recognizedFoods.Count > 0 ? recognizedFoods.Average(f => f.Calories) : 0,
                AvgCarbs = recognizedFoods.Count > 0 ? recognizedFoods.Average(f => f.Carbs) : 0,
                AvgProtein = recognizedFoods.Count > 0 ? recognizedFoods.Average(f => f.Protein) : 0,
                AvgFats = recognizedFoods.Count > 0 ? recognizedFoods.Average(f => f.Fats) : 0,
                TopRecognizedFoods = recognizedFoods
                    .Select(f => new NutritionalFoodItem
                    {
                        FoodName = f.Name,
                        CategoryName = f.CategoryFood?.CategoryName ?? "Unknown",
                        TimesRecognized = f.RecognitionResults.Count,
                        Calories = f.Calories,
                        Carbs = f.Carbs,
                        Protein = f.Protein,
                        Fats = f.Fats
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .ToList()
            };

            return _pdfReportGenerator.GenerateNutritionalInsightsReport(data);
        }
        #endregion

        #region 6. AI Performance by Category Report
        public async Task<Result<byte[]>> GenerateAiPerformanceByCategoryReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating AI Performance by Category Report");

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false, ct: ct)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Count > 0).ToList();
            var allRecognitions = recognizedFoods.SelectMany(f => f.RecognitionResults).ToList();

            var categoryGroups = recognizedFoods
                .GroupBy(f => f.CategoryFood?.CategoryName ?? "Uncategorized")
                .Select(g =>
                {
                    var categoryRecognitions = g.SelectMany(f => f.RecognitionResults).ToList();
                    return new CategoryPerformanceItem
                    {
                        CategoryName = g.Key,
                        TotalRecognitions = categoryRecognitions.Count,
                        AvgConfidence = categoryRecognitions.Count > 0 ? categoryRecognitions.Average(r => r.Confidence_Score) : 0,
                        MinConfidence = categoryRecognitions.Count > 0 ? categoryRecognitions.Min(r => r.Confidence_Score) : 0,
                        MaxConfidence = categoryRecognitions.Count > 0 ? categoryRecognitions.Max(r => r.Confidence_Score) : 0,
                        UniqueFoodsRecognized = g.Count()
                    };
                })
                .OrderByDescending(x => x.TotalRecognitions)
                .ToList();

            var data = new AiPerformanceByCategoryData
            {
                TotalCategories = categoryGroups.Count,
                TotalRecognitions = allRecognitions.Count,
                OverallAvgConfidence = allRecognitions.Count > 0 ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                CategoryPerformances = categoryGroups
            };

            return _pdfReportGenerator.GenerateAiPerformanceByCategoryReport(data);
        }
        #endregion

        #region 7. Usage Trends Report
        public async Task<Result<byte[]>> GenerateUsageTrendsReportAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Generating Usage Trends Report");

            var usersResult = await _identityService.GetAllUsersAsync(ct);
            var users = usersResult.IsSuccess
                ? usersResult.Value
                : Array.Empty<FoodRecognitionApp.Shared.Dtos.Auth.IdentityUserResult>();

            var usersMap = users.ToDictionary(u => u.Id);

            var imagesSpec = new AllImagesWithRecognitionsSpecification();
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false, ct: ct)).ToList();

            var allRecognitions = images.SelectMany(i => i.RecognitionResults ?? Enumerable.Empty<RecognitionResult>()).ToList();

            var data = new UsageTrendsData
            {
                TotalUsers = users.Count,
                TotalImages = images.Count,
                TotalRecognitions = allRecognitions.Count,
                MonthlyUsage = images
                    .GroupBy(i => i.UploadTime.ToString("yyyy-MM"))
                    .Select(g => new MonthlyUsage
                    {
                        Month = g.Key,
                        Images = g.Count(),
                        Recognitions = g.Sum(i => i.RecognitionResults?.Count ?? 0)
                    })
                    .OrderBy(x => x.Month)
                    .ToList(),
                DayOfWeekUsage = images
                    .GroupBy(i => i.UploadTime.DayOfWeek)
                    .Select(g => new DayOfWeekUsage
                    {
                        DayName = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .OrderBy(x => (int)Enum.Parse<DayOfWeek>(x.DayName))
                    .ToList(),
                MostActiveUsers = images
                    .GroupBy(i => i.UserId)
                    .Select(g =>
                    {
                        usersMap.TryGetValue(g.Key, out var user);
                        return new ActiveUserItem
                        {
                            UserName = user?.UserName ?? "Unknown",
                            Email = user?.Email ?? "Unknown",
                            ImagesUploaded = g.Count(),
                            FoodsRecognized = g.Sum(i => i.RecognitionResults?.Count ?? 0),
                            LastActivity = g.Max(i => (DateTime?)i.UploadTime)
                        };
                    })
                    .OrderByDescending(x => x.ImagesUploaded)
                    .Take(10)
                    .ToList()
            };

            return _pdfReportGenerator.GenerateUsageTrendsReport(data);
        }
        #endregion
    }
}
