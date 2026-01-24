using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class UserNotFoundException(string email) : NotFoundException($"User with email {email} was Not Found!!")
    {

    }
}
