using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class ProfileNotFoundException(string email) : NotFoundException($"Profile for User with Email {email} was Not Found!!") 
    {
    }
}
