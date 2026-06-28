using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Services.Abstraction.Reports;
using FoodRecognitionApp.Services.Reports.Documents;
using FoodRecognitionApp.Services.Reports.Models;
using FoodRecognitionApp.Services.Specifications;
using Microsoft.AspNetCore.Identity;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodRecognitionApp.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<UserAccount> _userManager;

        public ReportService(IUnitOfWork unitOfWork, UserManager<UserAccount> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        #region 1. System Overview Report
        public async Task<byte[]> GenerateSystemOverviewReportAsync()
        {
            var users = _userManager.Users.ToList();
            var categoriesSpec = new AllCategoriesWithFoodsSpecification();
            var categories = (await _unitOfWork.GetRepository<int, Category>().GetAllAsync(categoriesSpec, changeTracker: false)).ToList();
            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false)).ToList();
            var imagesSpec = new AllImagesWithRecognitionsSpecification();
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false)).ToList();

            var allRecognitions = images.SelectMany(i => i.RecognitionResults).ToList();

            var data = new SystemOverviewData
            {
                TotalUsers = users.Count,
                TotalImages = images.Count,
                TotalRecognitions = allRecognitions.Count,
                TotalFoods = foods.Count,
                TotalCategories = categories.Count,
                AvgConfidence = allRecognitions.Any() ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                FoodsByCategory = categories.Select(c => new CategoryFoodCount
                {
                    CategoryName = c.CategoryName,
                    FoodCount = c.Foods?.Count ?? 0
                }).Where(x => x.FoodCount > 0).ToList(),
                TopRecognizedFoods = foods
                    .Where(f => f.RecognitionResults != null && f.RecognitionResults.Any())
                    .Select(f => new FoodRecognitionCount
                    {
                        FoodName = f.Name,
                        TimesRecognized = f.RecognitionResults.Count
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .Take(10)
                    .ToList()
            };

            var document = new SystemOverviewDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 2. Food Recognition Analysis Report
        public async Task<byte[]> GenerateFoodRecognitionReportAsync()
        {
            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Any()).ToList();
            var allRecognitions = recognizedFoods.SelectMany(f => f.RecognitionResults).ToList();

            var data = new FoodRecognitionReportData
            {
                TotalRecognitions = allRecognitions.Count,
                AvgConfidence = allRecognitions.Any() ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                MinConfidence = allRecognitions.Any() ? allRecognitions.Min(r => r.Confidence_Score) : 0,
                MaxConfidence = allRecognitions.Any() ? allRecognitions.Max(r => r.Confidence_Score) : 0,
                TopFoodsByConfidence = recognizedFoods
                    .Select(f => new FoodConfidenceItem
                    {
                        FoodName = f.Name,
                        CategoryName = f.CategoryFood.CategoryName,
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
                        CategoryName = f.CategoryFood.CategoryName,
                        TimesRecognized = f.RecognitionResults.Count,
                        AvgConfidence = f.RecognitionResults.Average(r => r.Confidence_Score),
                        MinConfidence = f.RecognitionResults.Min(r => r.Confidence_Score),
                        MaxConfidence = f.RecognitionResults.Max(r => r.Confidence_Score)
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .ToList()
            };

            var document = new FoodRecognitionDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 3. User Demographics Report
        public async Task<byte[]> GenerateUserDemographicsReportAsync()
        {
            var profilesSpec = new AllUserProfilesSpecification();
            var profiles = (await _unitOfWork.GetRepository<int, UserProfile>().GetAllAsync(profilesSpec, changeTracker: false)).ToList();

            var data = new UserDemographicsData
            {
                TotalUsers = profiles.Count,
                MaleCount = profiles.Count(p => p.Gender == Gender.Male),
                FemaleCount = profiles.Count(p => p.Gender == Gender.Female),
                AvgAge = profiles.Any() ? profiles.Average(p => p.Age) : 0,
                AvgWeight = profiles.Any() ? (double)profiles.Average(p => p.Weight) : 0,
                AvgHeight = profiles.Any() ? (double)profiles.Average(p => p.Height) : 0,
                MaintainWeightCount = profiles.Count(p => p.GoalType == GoalType.MaintainWeight),
                LoseWeightCount = profiles.Count(p => p.GoalType == GoalType.LoseWeight),
                GainWeightCount = profiles.Count(p => p.GoalType == GoalType.GainWeight),
                SedentaryCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.Sedentary),
                LightlyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.LightlyActive),
                ModeratelyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.ModeratelyActive),
                HighlyActiveCount = profiles.Count(p => p.ActivityLevel == ActivityLevel.HighlyActive),
                UserDetails = profiles.Select(p => new UserProfileDetail
                {
                    UserName = p.UserAccount?.UserName ?? "N/A",
                    Email = p.UserAccount?.Email ?? "N/A",
                    Age = p.Age,
                    Weight = p.Weight,
                    Height = p.Height,
                    Gender = p.Gender.ToString(),
                    GoalType = p.GoalType.ToString(),
                    ActivityLevel = p.ActivityLevel.ToString(),
                    DailyCalories = p.Daily_Calories
                }).ToList()
            };

            var document = new UserDemographicsDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 4. User Recognition History Report
        public async Task<byte[]> GenerateUserRecognitionHistoryReportAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var imagesSpec = new UserImagesWithRecognitionsSpecification(userId);
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false)).ToList();

            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false)).ToList();
            var foodDict = foods.ToDictionary(f => f.Id);

            var allRecognitions = images.SelectMany(i => i.RecognitionResults.Select(r => new { Image = i, Recognition = r })).ToList();

            var data = new UserRecognitionHistoryData
            {
                UserName = user?.UserName ?? "Unknown",
                Email = user?.Email ?? "Unknown",
                TotalImages = images.Count,
                TotalRecognitions = allRecognitions.Count,
                AvgConfidence = allRecognitions.Any() ? allRecognitions.Average(r => r.Recognition.Confidence_Score) : 0,
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

            var document = new UserRecognitionHistoryDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 5. Nutritional Insights Report
        public async Task<byte[]> GenerateNutritionalInsightsReportAsync()
        {
            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Any()).ToList();

            var data = new NutritionalInsightsData
            {
                TotalRecognitions = recognizedFoods.Sum(f => f.RecognitionResults.Count),
                AvgCalories = recognizedFoods.Any() ? recognizedFoods.Average(f => f.Calories) : 0,
                AvgCarbs = recognizedFoods.Any() ? recognizedFoods.Average(f => f.Carbs) : 0,
                AvgProtein = recognizedFoods.Any() ? recognizedFoods.Average(f => f.Protein) : 0,
                AvgFats = recognizedFoods.Any() ? recognizedFoods.Average(f => f.Fats) : 0,
                TopRecognizedFoods = recognizedFoods
                    .Select(f => new NutritionalFoodItem
                    {
                        FoodName = f.Name,
                        CategoryName = f.CategoryFood.CategoryName,
                        TimesRecognized = f.RecognitionResults.Count,
                        Calories = f.Calories,
                        Carbs = f.Carbs,
                        Protein = f.Protein,
                        Fats = f.Fats
                    })
                    .OrderByDescending(x => x.TimesRecognized)
                    .ToList()
            };

            var document = new NutritionalInsightsDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 6. AI Performance by Category Report
        public async Task<byte[]> GenerateAiPerformanceByCategoryReportAsync()
        {
            var foodsSpec = new AllFoodsWithRecognitionsSpecification();
            var foods = (await _unitOfWork.GetRepository<int, Food>().GetAllAsync(foodsSpec, changeTracker: false)).ToList();

            var recognizedFoods = foods.Where(f => f.RecognitionResults != null && f.RecognitionResults.Any()).ToList();
            var allRecognitions = recognizedFoods.SelectMany(f => f.RecognitionResults).ToList();

            var categoryGroups = recognizedFoods
                .GroupBy(f => f.CategoryFood.CategoryName)
                .Select(g =>
                {
                    var categoryRecognitions = g.SelectMany(f => f.RecognitionResults).ToList();
                    return new CategoryPerformanceItem
                    {
                        CategoryName = g.Key,
                        TotalRecognitions = categoryRecognitions.Count,
                        AvgConfidence = categoryRecognitions.Average(r => r.Confidence_Score),
                        MinConfidence = categoryRecognitions.Min(r => r.Confidence_Score),
                        MaxConfidence = categoryRecognitions.Max(r => r.Confidence_Score),
                        UniqueFoodsRecognized = g.Count()
                    };
                })
                .OrderByDescending(x => x.TotalRecognitions)
                .ToList();

            var data = new AiPerformanceByCategoryData
            {
                TotalCategories = categoryGroups.Count,
                TotalRecognitions = allRecognitions.Count,
                OverallAvgConfidence = allRecognitions.Any() ? allRecognitions.Average(r => r.Confidence_Score) : 0,
                CategoryPerformances = categoryGroups
            };

            var document = new AiPerformanceByCategoryDocument(data);
            return document.GeneratePdf();
        }
        #endregion

        #region 7. Usage Trends Report
        public async Task<byte[]> GenerateUsageTrendsReportAsync()
        {
            var users = _userManager.Users.ToList();
            var imagesSpec = new AllImagesWithRecognitionsSpecification();
            var images = (await _unitOfWork.GetRepository<int, Image>().GetAllAsync(imagesSpec, changeTracker: false)).ToList();

            var allRecognitions = images.SelectMany(i => i.RecognitionResults).ToList();

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
                        var user = users.FirstOrDefault(u => u.Id == g.Key);
                        return new ActiveUserItem
                        {
                            UserName = user?.UserName ?? "Unknown",
                            Email = user?.Email ?? "Unknown",
                            ImagesUploaded = g.Count(),
                            FoodsRecognized = g.Sum(i => i.RecognitionResults?.Count ?? 0),
                            LastActivity = g.Max(i => i.UploadTime)
                        };
                    })
                    .OrderByDescending(x => x.ImagesUploaded)
                    .Take(10)
                    .ToList()
            };

            var document = new UsageTrendsDocument(data);
            return document.GeneratePdf();
        }
        #endregion
    }
}
