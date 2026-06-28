namespace FoodRecognitionApp.Services.Reports.Models
{
    public class UserRecognitionHistoryData
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TotalImages { get; set; }
        public int TotalRecognitions { get; set; }
        public double AvgConfidence { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<RecognitionHistoryItem> Recognitions { get; set; } = new();
        public List<MonthlyRecognitionCount> MonthlyTrend { get; set; } = new();
    }

    public class RecognitionHistoryItem
    {
        public DateTime UploadTime { get; set; }
        public string FoodName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public double Confidence { get; set; }
    }

    public class MonthlyRecognitionCount
    {
        public string Month { get; set; } = null!;
        public int Count { get; set; }
    }
}
