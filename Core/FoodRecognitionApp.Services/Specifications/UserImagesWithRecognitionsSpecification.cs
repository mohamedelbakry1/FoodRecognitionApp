using FoodRecognitionApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Specifications
{
    internal class UserImagesWithRecognitionsSpecification : BaseSpecification<int, Image>
    {
        public UserImagesWithRecognitionsSpecification(Guid userId) : base(i => i.UserId == userId)
        {
            Includes.Add(i => i.RecognitionResults);
            AddOrderByDescending(i => i.UploadTime);
        }
    }
}
