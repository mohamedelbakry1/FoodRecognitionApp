using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class MealResponse
    {
        public int MealId { get; set; }
        public string MealType { get; set; } = null!;
        public DateTime MealTime { get; set; }
        public decimal TotalCalories { get; set; }
        public ICollection<MealItemResponse> Items { get; set; } = null!;
    }
}
