using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class DailySummaryResponse
    {
        public string Date { get; set; } = null!;
        public decimal CaloriesTarget { get; set; }
        public decimal CaloriesConsumed { get; set; }
        public decimal CaloriesRemaining { get; set; }
        public decimal GoalPercentage { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFats { get; set; }
    }
}
