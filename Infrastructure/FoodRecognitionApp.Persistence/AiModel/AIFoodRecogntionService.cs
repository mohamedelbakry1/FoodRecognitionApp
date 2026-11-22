using FoodRecognitionApp.Services.Abstraction.AIModel;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.AIModel;
using FoodRecognitionApp.Shared.Dtos.FoodRecognition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FoodRecognitionApp.Persistence.AiModel
{
    internal class AIFoodRecogntionService(HttpClient _httpClient, ILogger<AIFoodRecogntionService> _logger) : IAIModelService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public async Task<Result<ICollection<AIModelResponse>>> ClassifyFoodAsync(IFormFile image, CancellationToken ct = default)
        {
            if (image is null || image.Length == 0)
                return Error.Validation("AiModel.EmptyImage", "Image cannot be null or empty.");
            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = image.OpenReadStream();

                if (stream.CanSeek)
                    stream.Position = 0;
                using var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                content.Add(streamContent, "file", image.FileName);

                _logger.LogInformation("Sending image {FileName} to AI model for analysis...", image.FileName);

                var response = await _httpClient.PostAsync("/analyze_meal", content, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("AI model returned non-success status: {StatusCode}", response.StatusCode);
                    return Error.Failure("AiModel.Unavailable", "AI service is currently unavailable or sleeping. Please try again shortly.");
                }
                var json = await response.Content.ReadAsStringAsync(ct);
                var aiResponse = JsonSerializer.Deserialize<AIModelApiResponse>(json, JsonOptions);

                if (aiResponse?.MealDetails is null || !aiResponse.MealDetails.Any())
                {
                    _logger.LogWarning("AI model succeeded but returned no meal details.");
                    return Error.NotFound("AiModel.NoFoodDetected", "No recognizable food items were detected in the image.");
                }

                var results = aiResponse.MealDetails.Select(item => new AIModelResponse
                {
                    FoodName = item.FoodName,
                    Confidence_Score = item.Confidence,
                    EstimatedWeightGrams = item.EstimatedWeightGrams
                }).ToList();

                _logger.LogInformation("AI model recognized {Count} food items.", results.Count);

                return results;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || !ex.CancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Timeout while waiting for AI model response.");
                return Error.Failure("AiModel.Timeout", "AI service took too long to respond. Please try again."); ;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error occurred while calling AI model API.");
                return Error.Failure("AiModel.Unavailable", "AI service is currently unavailable or sleeping. Please try again shortly.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI model JSON response.");
                return Error.Failure("AiModel.ParseError", "Failed to parse AI response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during food classification.");
                return Error.Failure("AiModel.Unexpected", $"AI Model error: {ex.Message}");
            }
        }
    }
}
