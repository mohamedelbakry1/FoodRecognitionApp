using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class ProfileByUserIdSpecification : BaseSpecification<int, UserProfile>
    {
        public ProfileByUserIdSpecification(Guid userId) : base(P => P.AccountId == userId)
        {

        }
    }
}
