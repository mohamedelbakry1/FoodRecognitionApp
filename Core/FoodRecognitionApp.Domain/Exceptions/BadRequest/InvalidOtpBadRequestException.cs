using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class InvalidOtpBadRequestException() : BadRequestException("Invalid or Expired OTP !!")
    {
    }
}
