using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FoodRecognitionApp.Shared.Dtos.AIModel
{
    public class AIModelApiResponse
    {
        [JsonPropertyName("meal_details")]
        public List<MealDetailItem> MealDetails { get; set; } = null!;
    }
}
