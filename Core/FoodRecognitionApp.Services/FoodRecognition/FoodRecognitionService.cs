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
                var recognition = image.RecognitionResults.FirstOrDefault();
                if (recognition is null) throw new RecognitionResultNotFoundException();

                var foodspec = new FoodByIdSpecification(recognition.FoodId);
                var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodspec);

                if (food is null) throw new FoodNotFoundException(food!.Name);

                result.Add(new RecentRecognitionResponse
                {
                    FoodName = food.Name,
                    ImageUrl = $"{baseUrl}/images/{image.ImageUrl}",
                    UploadTime = image.UploadTime
                });
            }

            return result;
        }

        public async Task<IEnumerable<FoodRecognitionResponse>?> RecognizeFoodAsync(int userId, FoodRecognitionRequest request)
        {
            if (request.Image is null || request.Image.Length == 0) throw new UploadImageBadRequestException();

            var imageUrl = await _attachmentService.Upload(ImageFolderName, request.Image);

            if (imageUrl is null) throw new UploadImageBadRequestException();

            var image = new Image()
            {
                ImageUrl = imageUrl,
                UserId = userId,
                UploadTime = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<int, Image>().AddAsync(image);

            var AiItems = await _aiModelService.ClassifyFoodAsync(request.Image);

            if(AiItems is null || !AiItems.Any()) throw new FoodRecognitionBadRequestException();

            var recognitionresult = new List<RecognitionResult>();
            var foodResponses = new List<FoodRecognitionResponse>();

            foreach (var AiItem in AiItems)
            {
                var foodSpec = new FoodByNameSpecification(AiItem!.FoodName);
                var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec);
                if (food is null) continue;

                recognitionresult.Add(new RecognitionResult
                {
                    ImageId = image.Id,
                    FoodId = food.Id,
                    Confidence_Score = AiItem.Confidence_Score,
                    Estimated_Quantity = AiItem.EstimatedWeightGrams
                });

                foodResponses.Add(new FoodRecognitionResponse
                {
                    FoodId = food.Id,
                    FoodName = food.Name,
                    Calories = (food.Calories / 100m) * AiItem.EstimatedWeightGrams,
                    Carbs = (food.Carbs / 100m) * AiItem.EstimatedWeightGrams,
                    Protein = (food.Protein / 100m) * AiItem.EstimatedWeightGrams,
                    Fats = (food.Fats / 100m) * AiItem.EstimatedWeightGrams,
                    CategoryName = food.CategoryFood.CategoryName,
                    Confidence_Score = AiItem.Confidence_Score,
                    EstimatedWeightGrams = AiItem.EstimatedWeightGrams
                });
            }

            if (!recognitionresult.Any()) throw new FoodRecognitionBadRequestException();

            image.RecognitionResults = recognitionresult;

            var IsCreated = await _unitOfWork.SaveChangesAsync() > 0;

            if (!IsCreated) _attachmentService.Delete(ImageFolderName, imageUrl);
            
            return foodResponses;
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
