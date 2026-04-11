using FoodRecognitionApp.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Meal
{
    public class UpdateMealTypeRequest
    {
        [Required(ErrorMessage = "Meal type is required")]
        [RegularExpression("^(Breakfast|Lunch|Dinner|Snack)$",
    ErrorMessage = "Meal type must be Breakfast, Lunch, Dinner, or Snack")]
        public MealType MealType { get; set; }
    }
}
