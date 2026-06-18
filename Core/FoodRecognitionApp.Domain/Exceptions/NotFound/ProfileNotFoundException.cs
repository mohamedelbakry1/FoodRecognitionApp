using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class ProfileNotFoundException(int userId) : NotFoundException($"Profile for User with UserId {userId} was Not Found!!") 
    {
    }
}
