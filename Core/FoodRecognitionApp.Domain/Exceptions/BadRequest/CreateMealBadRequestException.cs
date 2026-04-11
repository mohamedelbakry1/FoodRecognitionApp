using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class CreateMealBadRequestException() : BadRequestException("Failed to Create the Meal, Please try again")
    {
    }
}
