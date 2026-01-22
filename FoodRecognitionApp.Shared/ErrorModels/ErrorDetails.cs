using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.ErrorModels
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = null!;
    }
}
