using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.FoodRecognition
{
    public class FoodRecognitionResponse
    {
        public string FoodName { get; set; } = null!;
        public decimal TotalCalories { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFats { get; set; }
    }
}
