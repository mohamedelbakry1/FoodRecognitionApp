using System;
using System.Collections.Generic;

namespace FoodRecognitionApp.Services.Abstraction.Reports.Models
{
    public class NutritionalInsightsData
    {
        public int TotalRecognitions { get; set; }
        public decimal AvgCalories { get; set; }
        public decimal AvgCarbs { get; set; }
        public decimal AvgProtein { get; set; }
        public decimal AvgFats { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<NutritionalFoodItem> TopRecognizedFoods { get; set; } = new();
    }

    public class NutritionalFoodItem
    {
        public string FoodName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int TimesRecognized { get; set; }
        public decimal Calories { get; set; }
        public decimal Carbs { get; set; }
        public decimal Protein { get; set; }
        public decimal Fats { get; set; }
    }
}
