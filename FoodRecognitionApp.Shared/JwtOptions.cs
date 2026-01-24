using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SecurityKey { get; set; } = null!;
        public double DurationInDays { get; set; }
    }
}
