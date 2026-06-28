namespace FoodRecognitionApp.Services.Reports.Models
{
    public class UsageTrendsData
    {
        public int TotalUsers { get; set; }
        public int TotalRecognitions { get; set; }
        public int TotalImages { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<MonthlyUsage> MonthlyUsage { get; set; } = new();
        public List<DayOfWeekUsage> DayOfWeekUsage { get; set; } = new();
        public List<ActiveUserItem> MostActiveUsers { get; set; } = new();
    }

    public class MonthlyUsage
    {
        public string Month { get; set; } = null!;
        public int Recognitions { get; set; }
        public int Images { get; set; }
    }

    public class DayOfWeekUsage
    {
        public string DayName { get; set; } = null!;
        public int Count { get; set; }
    }

    public class ActiveUserItem
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ImagesUploaded { get; set; }
        public int FoodsRecognized { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
