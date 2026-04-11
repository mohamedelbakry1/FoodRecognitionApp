using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class TodayMealsResponse
    {
        public string Date { get; set; } = null!;
        public decimal TotalCalories { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalProteins { get; set; }
        public decimal TotalFats { get; set; }
        public IEnumerable<MealResponse> Meals { get; set; } = null!;
    }
}
