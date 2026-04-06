using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class RemovePasswordBadRequestException() : BadRequestException("Failed to Reset Password , Please Try Again !!")
    {
    }
}
