using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
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
        IWebHostEnvironment _webHostEnvironment
        ) : IFoodRecognitionService
    {
        public async Task<FoodRecognitionResponse?> RecognizeFoodAsync(int userId, FoodRecognitionRequest request)
        {
            if (request.Image is null || request.Image.Length == 0) throw new UploadImageBadRequestException();

            var imageUrl = await SaveImageAsync(request.Image);

            var image = new Image()
            {
                ImageUrl = imageUrl,
                UserId = userId,
                UploadTime = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<int, Image>().AddAsync(image);

            var aiResponse = GetAiResponse();

            var foodSpec = new FoodByNameSpecification(aiResponse.FoodName);
            var food = await _unitOfWork.GetRepository<int, Food>().GetById(foodSpec);

            if(food is null) throw new FoodNotFoundException(aiResponse.FoodName);

            var recognitionResult = new RecognitionResult()
            {
                ImageId = image.Id,
                FoodId = food.Id,
                Confidence_Score = aiResponse.Confidence_Score,
                Estimated_Quantity = 1.0m
            };

            image.RecognitionResults = new List<RecognitionResult> { recognitionResult };

            await _unitOfWork.SaveChangesAsync();

            return new FoodRecognitionResponse
            {
                FoodName = food.Name,
                Calories = food.Calories,
                Carbs = food.Carbs,
                Protien = food.Protein,
                Fats = food.Fats,
                CategoryName = food.CategoryFood.CategoryName,
                Confidence_Score = aiResponse.Confidence_Score,
            };
        }


        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath,"images");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var UniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";

            var filePath = Path.Combine(uploadFolder, UniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(fileStream);

            return $"/images/{UniqueFileName}";
        }

        private AIModelResponse GetAiResponse()
        {
            var ranFoods = new[]
{
                new { Name = "Pizza", Confidence = 0.95 },
                new { Name = "Hamburger", Confidence = 0.89 },
                new { Name = "Sushi", Confidence = 0.92 },
                new { Name = "Ice Cream", Confidence = 0.88 },
                new { Name = "French Fries", Confidence = 0.91 }
            };
            var random = new Random();
            var selected = ranFoods[random.Next(ranFoods.Length)];
            return new AIModelResponse
            {
                FoodName = selected.Name,
                Confidence_Score = selected.Confidence
            };
        }
    }
}
