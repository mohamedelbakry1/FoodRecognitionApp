namespace FoodRecognitionApp.Services.Reports.Models
{
    public class UserDemographicsData
    {
        public int TotalUsers { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public double AvgAge { get; set; }
        public double AvgWeight { get; set; }
        public double AvgHeight { get; set; }
        public int MaintainWeightCount { get; set; }
        public int LoseWeightCount { get; set; }
        public int GainWeightCount { get; set; }
        public int SedentaryCount { get; set; }
        public int LightlyActiveCount { get; set; }
        public int ModeratelyActiveCount { get; set; }
        public int HighlyActiveCount { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<UserProfileDetail> UserDetails { get; set; } = new();
    }

    public class UserProfileDetail
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public string Gender { get; set; } = null!;
        public string GoalType { get; set; } = null!;
        public string ActivityLevel { get; set; } = null!;
        public decimal DailyCalories { get; set; }
    }
}
