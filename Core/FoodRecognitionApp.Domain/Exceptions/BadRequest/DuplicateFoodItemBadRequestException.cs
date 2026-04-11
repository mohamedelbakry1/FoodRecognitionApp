using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class DuplicateFoodItemBadRequestException(int foodId) : BadRequestException($"Duplicate Food item with Id: {foodId}, Use Quantity Instead")
    {
    }
}
