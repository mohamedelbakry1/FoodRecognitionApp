using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FoodRecognitionApp.Shared.Dtos.AIModel
{
    public class AIModelRequest
    {
        [JsonPropertyName("class_id")]
        public int ClassId { get; set; }
        [JsonPropertyName("class_name")]
        public string ClassName { get; set; } = null!;
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
