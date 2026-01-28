using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class ProfileByUserIdSpecification : BaseSpecification<int, UserProfile>
    {
        public ProfileByUserIdSpecification(int userId) : base(P => P.AccountId == userId)
        {
            Includes.Add(P => P.UserAccount);
        }
    }
}
