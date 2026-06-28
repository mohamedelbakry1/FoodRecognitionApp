using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    public class AllFoodsWithRecognitionsSpecification : BaseSpecification<int, Food>
    {
        public AllFoodsWithRecognitionsSpecification() : base(f => true)
        {
            Includes.Add(f => f.CategoryFood);
            Includes.Add(f => f.RecognitionResults);
            AddOrderBy(f => f.Name);
        }
    }
}
