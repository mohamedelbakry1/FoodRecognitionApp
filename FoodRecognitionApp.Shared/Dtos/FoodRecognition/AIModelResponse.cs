using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.FoodRecognition
{
    public class AIModelResponse
    {
        public string FoodName { get; set; } = null!;
        public double Confidence_Score { get; set; }
    }
}
