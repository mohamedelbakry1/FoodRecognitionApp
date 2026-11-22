using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class FoodByIdSpecification : BaseSpecification<int, Food>
    {
        public FoodByIdSpecification(int FoodId) : base(F => F.Id == FoodId)
        {
            
        }
    }
}
