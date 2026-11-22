using System;
using System.Collections.Generic;

namespace FoodRecognitionApp.Services.Abstraction.Reports.Models
{
    public class AiPerformanceByCategoryData
    {
        public int TotalCategories { get; set; }
        public int TotalRecognitions { get; set; }
        public double OverallAvgConfidence { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<CategoryPerformanceItem> CategoryPerformances { get; set; } = new();
    }

    public class CategoryPerformanceItem
    {
        public string CategoryName { get; set; } = null!;
        public int TotalRecognitions { get; set; }
        public double AvgConfidence { get; set; }
        public double MinConfidence { get; set; }
        public double MaxConfidence { get; set; }
        public int UniqueFoodsRecognized { get; set; }
    }
}
