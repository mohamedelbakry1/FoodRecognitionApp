using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Profile
{
    public class CreateProfileResponse
    {
        public decimal AMR { get; set; }
        public decimal BMR { get; set; }
        public decimal DailyCaloriesTarget { get; set; }
    }
}
