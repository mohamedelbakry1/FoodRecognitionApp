using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class RecognitionResultNotFoundException() : NotFoundException("Recognition Result Not Found in Database")
    {

    }
}
