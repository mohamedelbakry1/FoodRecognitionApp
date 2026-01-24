using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class CreateProfileBadRequestException() : BadRequestException("Invalid Operation when Create Profile !!")
    {

    }
}
