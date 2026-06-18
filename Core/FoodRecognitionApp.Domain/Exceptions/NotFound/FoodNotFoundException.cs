using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class FoodNotFoundException() : NotFoundException($"Food Not Found in Database")
    {

    }
}
