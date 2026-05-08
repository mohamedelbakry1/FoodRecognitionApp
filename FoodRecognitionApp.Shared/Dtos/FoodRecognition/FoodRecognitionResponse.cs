using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.FoodRecognition
{
    public class FoodRecognitionResponse
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = null!;
        public decimal Calories { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fats { get; set; }
        public decimal Protein { get; set; }
        public string CategoryName { get; set; } = null!;
        public double Confidence_Score { get; set; }
        public decimal EstimatedWeightGrams { get; set; }
    }
}
