using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.BadRequest
{
    public class UploadImageBadRequestException() : BadRequestException("Uploaded image is Failed Please Try again")
    {
    }
}
