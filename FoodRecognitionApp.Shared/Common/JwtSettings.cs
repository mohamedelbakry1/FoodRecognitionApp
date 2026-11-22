using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Common
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SecurityKey { get; set; } = null!;
        public double DurationInDays { get; set; }
    }
}
