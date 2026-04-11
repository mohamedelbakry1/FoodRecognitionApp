using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class DeleteMealBadRequestException() : BadRequestException("Failed to Delete the Meal, Please try again")
    {

    }
}
