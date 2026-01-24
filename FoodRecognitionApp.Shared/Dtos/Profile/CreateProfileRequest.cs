using FoodRecognitionApp.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Profile
{
    public class CreateProfileRequest
    {
        [Required(ErrorMessage ="Age is Required")]
        [Range(3,100,ErrorMessage = "Age must be between 16 and 100 years")]
        public int Age { get; set; }
        [Required(ErrorMessage ="Height is Required")]
        [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm")]
        public decimal Height { get; set; }
        [Required(ErrorMessage = "Weight is required")]
        [Range(30, 300, ErrorMessage = "Weight must be between 30 and 300 kg")]
        public decimal Weight { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either Male or Female")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Goal type is required")]
        [RegularExpression("^(MaintainWeight|LoseWeight|GainWeight)$",
        ErrorMessage = "Goal type must be MaintainWeight, LoseWeight, or GainWeight")]
        public GoalType GoalType { get; set; }
        [Required(ErrorMessage = "Activity level is required")]
        [RegularExpression("^(Sedentary|LightlyActive|ModeratelyActive|ExtraActive|VeryActive)$",
        ErrorMessage = "Activity level must be Sedentary, LightlyActive, ModeratelyActive, ExtraActive, or VeryActive")]
        public ActivityLevel ActivityLevel { get; set; }
    }
}
