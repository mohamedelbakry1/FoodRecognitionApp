using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.FoodRecognition
{
    public class FoodRecognitionRequest
    {
        public IFormFile Image { get; set; } = null!;
    }
}
