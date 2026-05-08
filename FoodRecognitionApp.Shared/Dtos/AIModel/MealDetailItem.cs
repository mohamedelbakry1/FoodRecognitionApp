using System.Text.Json.Serialization;

namespace FoodRecognitionApp.Shared.Dtos.AIModel
{
    public class MealDetailItem
    {
        [JsonPropertyName("food_name")]
        public string FoodName { get; set; } = null!;
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
        [JsonPropertyName("estimated_weight_grams")]
        public decimal EstimatedWeightGrams { get; set; }
    }
}