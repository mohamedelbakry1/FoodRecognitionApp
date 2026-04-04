using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.FoodRecognition
{
    public class RecentRecognitionResponse
    {
        public string FoodName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime UploadTime { get; set; }
        public string TimeDisplay => $"{UploadTime:hh:mm}";
    }
}
