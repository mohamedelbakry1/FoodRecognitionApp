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
        public async Task<AIModelResponse?> ClassifyFoodAsync(IFormFile image)
        {
            using var content = new MultipartFormDataContent();
            using var stream = image.OpenReadStream();
            using var straemContent = new StreamContent(stream);

            straemContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

            content.Add(straemContent, "file", image.FileName);

            var response = await _httpClient.PostAsync("/predict", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var AiResponse = JsonSerializer.Deserialize<AIModelRequest>(json);

            if (AiResponse is null) return null;
            return new AIModelResponse
            {
                FoodName = AiResponse.ClassName,
                Confidence_Score = AiResponse.Confidence
            };
        }
    }
}
