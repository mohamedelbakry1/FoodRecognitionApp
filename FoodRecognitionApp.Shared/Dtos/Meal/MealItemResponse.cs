using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class MealItemResponse
    {
        public string FoodName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fats { get; set; }
        public decimal Protein { get; set; }
        public decimal Item_Calories { get; set; }
    }
}
