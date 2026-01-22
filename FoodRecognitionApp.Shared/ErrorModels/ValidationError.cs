using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.ErrorModels
{
    public class ValidationError
    {
        public string Field { get; set; } = null!;
        public ICollection<string> Errors { get; set; } = null!;
    }
}
