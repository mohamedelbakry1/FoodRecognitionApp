namespace FoodRecognitionApp.Services.Reports.Models
{
    public class SystemOverviewData
    {
        public int TotalUsers { get; set; }
        public int TotalImages { get; set; }
        public int TotalRecognitions { get; set; }
        public int TotalFoods { get; set; }
        public int TotalCategories { get; set; }
        public double AvgConfidence { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<CategoryFoodCount> FoodsByCategory { get; set; } = new();
        public List<FoodRecognitionCount> TopRecognizedFoods { get; set; } = new();
    }

    public class CategoryFoodCount
    {
        public string CategoryName { get; set; } = null!;
        public int FoodCount { get; set; }
    }

    public class FoodRecognitionCount
    {
        public string FoodName { get; set; } = null!;
        public int TimesRecognized { get; set; }
    }
}
