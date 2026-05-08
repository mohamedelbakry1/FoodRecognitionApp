using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Shared.Dtos.AIModel;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodRecognitionApp.Services.AIModel
{
    public class AIModelService(HttpClient _httpClient) : IAIModelService
    {
        public async Task<IEnumerable<AIModelResponse>?> ClassifyFoodAsync(IFormFile image)
        {
            using var content = new MultipartFormDataContent();
            using var stream = image.OpenReadStream();
            using var straemContent = new StreamContent(stream);

            straemContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

            content.Add(straemContent, "file", image.FileName);

            var response = await _httpClient.PostAsync("/analyze_meal", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var AiResponse = JsonSerializer.Deserialize<AIModelApiResponse>(json, options);

            if (AiResponse is null || AiResponse.MealDetails is null || !AiResponse.MealDetails.Any())
                return null;
            return AiResponse.MealDetails.Select(item => new AIModelResponse
            {
                FoodName = item.FoodName,
                Confidence_Score = item.Confidence,
                EstimatedWeightGrams = item.EstimatedWeightGrams
            }).ToList();
        }
    }
}
