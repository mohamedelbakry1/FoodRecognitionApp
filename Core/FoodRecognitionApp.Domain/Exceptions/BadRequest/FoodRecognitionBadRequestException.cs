using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class FoodRecognitionBadRequestException() : BadRequestException("Failed to Detect Food")
    {
    }
}
