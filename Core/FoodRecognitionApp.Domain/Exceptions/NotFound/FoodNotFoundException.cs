using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class FoodNotFoundException(string foodName) : NotFoundException($"Food {foodName} Not Found in Database")
    {

    }
}
