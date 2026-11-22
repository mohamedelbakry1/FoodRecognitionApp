using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FoodRecognitionApp.Shared.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        Unauthorized = 2,
        Forbidden = 3,
        NotFound = 4,
        Conflict = 5,
        InvalidCredentials = 6
    }
}
