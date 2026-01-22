using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Contracts
{
    public interface IDbIntializer
    {
        Task IntializeAsync();
    }
}
