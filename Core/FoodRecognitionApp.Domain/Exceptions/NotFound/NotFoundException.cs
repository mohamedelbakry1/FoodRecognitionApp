using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class NotFoundException(string message) : Exception(message)
    {

    }
}
