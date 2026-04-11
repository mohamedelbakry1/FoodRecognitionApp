using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Domain.Exceptions.NotFound
{
    public class MealNotFoundException(int mealId) : NotFoundException($"Meal with Id: {mealId} was Not found")
    {
    }
}
