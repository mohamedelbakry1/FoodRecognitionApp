using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class FoodItemResponse
    {
        public int FoodId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Calories { get; set; }
        public decimal Carbs { get; set; }
        public decimal Protein { get; set; }
        public decimal Fats { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
