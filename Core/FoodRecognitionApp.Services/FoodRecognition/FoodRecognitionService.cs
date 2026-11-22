using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Services.Abstraction.AttachmentService;
using FoodRecognitionApp.Services.Abstraction.FoodRecognition;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.FoodRecognition
{
    internal class FoodRecognitionService(
        IUnitOfWork _unitOfWork,
        IAttachmentService _attachmentService,
        IAIModelService _aiModelService,
        IHttpContextAccessor _httpContextAccessor
    ) : IFoodRecognitionService
    {
        private const string ImageFolderName = "Uploads/images";
        public async Task<Result<ICollection<RecentRecognitionResponse>>> GetRecentRecognitionsAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");
            var baseUrl = GetBaseUrl();
            var spec = new RecentFoodImagesSpecification(userId);
            var images = await _unitOfWork.GetRepository<int, Image>().GetAllAsync(spec, ct: ct);
            var result = new List<RecentRecognitionResponse>();
            foreach (var image in images)
            {
                var foodNames = new List<string>();
                foreach (var recognition in image.RecognitionResults)
                {
                    var foodSpec = new FoodByIdSpecification(recognition.FoodId);
                    var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec, ct: ct);
                    if (food is not null)
                        foodNames.Add(food.Name);
                }
                if (!foodNames.Any()) continue;
                result.Add(new RecentRecognitionResponse
                {
                    FoodName = string.Join(", ", foodNames),
                    ImageUrl = $"{baseUrl}/uploads/images/{image.ImageUrl}",
                    UploadTime = image.UploadTime
                });
            }
            return result;
        }
        public async Task<Result<FoodRecognitionResponse?>> RecognizeFoodAsync(Guid userId, FoodRecognitionRequest request, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                return Error.Validation("User.InvalidId", "User ID is invalid.");
            if (request is null || request.Image is null || request.Image.Length == 0)
                return Error.Validation("FoodRecognition.EmptyImage", "Image cannot be null or empty.");

            var uploadResult = await _attachmentService.Upload(ImageFolderName, request.Image, ct);
            if (uploadResult.IsFailure)
                return uploadResult.Errors.ToList();

            var imageUrl = uploadResult.Value;
            var image = new Image
            {
                ImageUrl = imageUrl,
                UserId = userId,
                UploadTime = DateTime.UtcNow
            };
            _unitOfWork.GetRepository<int, Image>().Add(image);

            var aiResult = await _aiModelService.ClassifyFoodAsync(request.Image, ct);
            if (aiResult.IsFailure)
            {
                _attachmentService.Delete(ImageFolderName, imageUrl);
                return aiResult.Errors.ToList();
            }
            var aiItems = aiResult.Value;

            if (aiItems is null || !aiItems.Any())
                return Error.Failure("FoodRecognition.ClassificationFailed","Unable to recognize any food from the provided image.");

            var recognitionResults = new List<RecognitionResult>();
            decimal totalCalories = 0, totalCarbs = 0, totalProtein = 0, totalFats = 0;
            var foodNames = new List<string>();

            foreach (var aiItem in aiItems)
            {
                var foodSpec = new FoodByNameSpecification(aiItem.FoodName);
                var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec, ct: ct);
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
                return Error.NotFound("FoodRecognition.NoFoodsRecognized","None of the recognized foods were found in the database.");

            image.RecognitionResults = recognitionResults;
            await _unitOfWork.SaveChangesAsync(ct);
            var response = new FoodRecognitionResponse
            {
                FoodName = string.Join(", ", foodNames),
                TotalCalories = totalCalories,
                TotalCarbs = totalCarbs,
                TotalProtein = totalProtein,
                TotalFats = totalFats
            };

            return response;
        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is null) return string.Empty;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
