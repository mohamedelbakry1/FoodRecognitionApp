using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.UnAuthorized
{
    public class UnAuthorizedException(string message) : Exception(message)
    {

    }
}
