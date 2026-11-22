using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class AllImagesWithRecognitionsSpecification : BaseSpecification<int, Image>
    {
        public AllImagesWithRecognitionsSpecification() : base(i => true)
        {
            Includes.Add(i => i.RecognitionResults);
            AddOrderByDescending(i => i.UploadTime);
        }
    }
}
