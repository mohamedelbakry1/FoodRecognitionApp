using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class UpdateMealBadRequestException() : BadRequestException("Failed to Update the Meal, Please try again")
    {
    }
}
