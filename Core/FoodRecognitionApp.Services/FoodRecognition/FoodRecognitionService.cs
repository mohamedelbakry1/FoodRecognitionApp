using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Services.Abstraction.AttachmentService;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.FoodRecognition
{
    public class FoodRecognitionService
        (IUnitOfWork _unitOfWork,
        IAttachmentService _attachmentService,
        IAIModelService _aiModelService,
        IHttpContextAccessor _httpContextAccessor
        ) : IFoodRecognitionService
    {
        private const string ImageFolderName = "images";
        public async Task<IEnumerable<RecentRecognitionResponse>> GetRecentRecognitionsAsync(int userId)
        {
            var baseUrl = GetBaseUrl();
            var spec = new RecentFoodImagesSpecification(userId);

            var images = await _unitOfWork.GetRepository<int, Image>().GetAllAsync(spec);

            var result = new List<RecentRecognitionResponse>();

            foreach(var image in images)
            {
                var foodNames = new List<string>();

                foreach (var recognition in image.RecognitionResults)
                {
                    var foodSpec = new FoodByIdSpecification(recognition.FoodId);
                    var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec);
                    if (food is not null) foodNames.Add(food.Name);
                }

                if (!foodNames.Any()) continue;

                result.Add(new RecentRecognitionResponse
                {
                    FoodName = string.Join(", ", foodNames),
                    ImageUrl = $"{baseUrl}/images/{image.ImageUrl}",
                    UploadTime = image.UploadTime
                });
            }

            return result;
        }

        public async Task<FoodRecognitionResponse?> RecognizeFoodAsync(int userId, FoodRecognitionRequest request)
        {
            if (request.Image is null || request.Image.Length == 0)
                throw new UploadImageBadRequestException();

            var imageUrl = await _attachmentService.Upload(ImageFolderName, request.Image);
            if (imageUrl is null) throw new UploadImageBadRequestException();

            var image = new Image()
            {
                ImageUrl = imageUrl,
                UserId = userId,
                UploadTime = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<int, Image>().AddAsync(image);

            var aiItems = await _aiModelService.ClassifyFoodAsync(request.Image);
            if (aiItems is null || !aiItems.Any()) throw new FoodRecognitionBadRequestException();

            var recognitionResults = new List<RecognitionResult>();
            decimal totalCalories = 0, totalCarbs = 0, totalProtein = 0, totalFats = 0;
            var foodNames = new List<string>();

            foreach (var aiItem in aiItems)
            {
                var foodSpec = new FoodByNameSpecification(aiItem.FoodName);
                var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec);

                if (food is null) continue;
                if (recognitionResults.Any(r => r.FoodId == food.Id)) continue;

                var calories = (food.Calories / 100m) * aiItem.EstimatedWeightGrams;
                var carbs = (food.Carbs / 100m) * aiItem.EstimatedWeightGrams;
                var protein = (food.Protein / 100m) * aiItem.EstimatedWeightGrams;
                var fats = (food.Fats / 100m) * aiItem.EstimatedWeightGrams;

                totalCalories += calories;
                totalCarbs += carbs;
                totalProtein += protein;
                totalFats += fats;
                foodNames.Add(food.Name);

                recognitionResults.Add(new RecognitionResult
                {
                    ImageId = image.Id,
                    FoodId = food.Id,
                    Confidence_Score = aiItem.Confidence_Score,
                    Estimated_Quantity = aiItem.EstimatedWeightGrams
                });
            }
            if (!recognitionResults.Any())
                throw new FoodRecognitionBadRequestException();

            image.RecognitionResults = recognitionResults;
            await _unitOfWork.SaveChangesAsync();

            return new FoodRecognitionResponse
            {
                FoodName = string.Join(", ", foodNames),
                TotalCalories = totalCalories,
                TotalCarbs = totalCarbs,
                TotalProtein = totalProtein,
                TotalFats = totalFats
            };
        }

        //private AIModelResponse GetAiResponse()
        //{
        //    var ranFoods = new[]
        //    {
        //        new { Name = "Pizza", Confidence = 0.95 },
        //        new { Name = "Hamburger", Confidence = 0.89 },
        //        new { Name = "Sushi", Confidence = 0.92 },
        //        new { Name = "Ice Cream", Confidence = 0.88 },
        //        new { Name = "French Fries", Confidence = 0.91 }
        //    };
        //    var random = new Random();
        //    var selected = ranFoods[random.Next(ranFoods.Length)];
        //    return new AIModelResponse
        //    {
        //        FoodName = selected.Name,
        //        Confidence_Score = selected.Confidence
        //    };
        //}

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
