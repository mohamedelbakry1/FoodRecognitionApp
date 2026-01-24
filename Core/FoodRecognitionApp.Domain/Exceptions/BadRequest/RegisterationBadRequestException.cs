using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class RegisterationBadRequestException(List<string> errors) : BadRequestException(string.Join(", ",errors))
    {

    }
}
