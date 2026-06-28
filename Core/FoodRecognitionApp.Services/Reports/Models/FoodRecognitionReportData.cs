namespace FoodRecognitionApp.Services.Reports.Models
{
    public class FoodRecognitionReportData
    {
        public int TotalRecognitions { get; set; }
        public double AvgConfidence { get; set; }
        public double MinConfidence { get; set; }
        public double MaxConfidence { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<FoodConfidenceItem> TopFoodsByConfidence { get; set; } = new();
        public List<FoodRecognitionCount> TopFoodsByCount { get; set; } = new();
        public List<FoodRecognitionDetail> AllFoodDetails { get; set; } = new();
    }

    public class FoodConfidenceItem
    {
        public string FoodName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public double AvgConfidence { get; set; }
        public int TimesRecognized { get; set; }
    }

    public class FoodRecognitionDetail
    {
        public string FoodName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int TimesRecognized { get; set; }
        public double AvgConfidence { get; set; }
        public double MinConfidence { get; set; }
        public double MaxConfidence { get; set; }
    }
}
