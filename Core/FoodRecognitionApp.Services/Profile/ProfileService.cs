using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Domain.Exceptions.BadRequest;
using FoodRecognitionApp.Domain.Exceptions.NotFound;
using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Profile
{
    public class ProfileService(IUnitOfWork _unitOfWork) : IProfileService
    {
        public async Task<CreateProfileResponse> CreateProfileAsync(int userId,CreateProfileRequest request)
        {
            // 1. Calculate BMR
            decimal bmr = 0;
            if (request.Gender == Gender.Male)
                bmr = (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) + 5;
            else
                bmr = (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) - 161;

            // 2. Calculate AMR
            decimal activityMultiplier = request.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2m,
                ActivityLevel.LightlyActive => 1.375m,
                ActivityLevel.ModeratelyActive => 1.55m,
                ActivityLevel.VeryActive => 1.725m,
                ActivityLevel.ExtraActive => 1.9m,
                _ => 1.2m
            };

            decimal amr = bmr * activityMultiplier;

            // 3. Calculate Daily Target based on Goal
            decimal dailyCalories = request.GoalType switch
            {
                GoalType.LoseWeight => amr - 500,
                GoalType.GainWeight => amr + 500,
                _ => amr // Maintain
            };

            var userProfile = new UserProfile
            {
                AccountId = userId,
                Age = request.Age,
                Weight = request.Weight,
                Height = request.Height,
                Gender = request.Gender,
                ActivityLevel = request.ActivityLevel,
                GoalType = request.GoalType,
                Daily_Calories = dailyCalories
            };

            await _unitOfWork.GetRepository<int, UserProfile>().AddAsync(userProfile);

            var count = await _unitOfWork.SaveChangesAsync();

            if (count <= 0) throw new CreateProfileBadRequestException();
            
            return new CreateProfileResponse
            {
                BMR = bmr,
                AMR = amr,
                DailyCaloriesTarget = dailyCalories
            };

        }

        public async Task UpdateProfileAsync(int userId, CreateProfileRequest request)
        {
            var spec = new ProfileByUserIdSpecification(userId);

            var profile = await _unitOfWork.GetRepository<int, UserProfile>().GetById(spec);

            if(profile == null) throw new ProfileNotFoundException(profile.UserAccount.Email);

            profile.Age = request.Age;
            profile.Weight = request.Weight;
            profile.Height = request.Height;
            profile.Gender = request.Gender;
            profile.ActivityLevel = request.ActivityLevel;
            profile.GoalType = request.GoalType;


            // 1. Calculate BMR
            decimal bmr = 0;
            if (request.Gender == Gender.Male)
                bmr = (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) + 5;
            else
                bmr = (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) - 161;

            // 2. Calculate AMR
            decimal activityMultiplier = request.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2m,
                ActivityLevel.LightlyActive => 1.375m,
                ActivityLevel.ModeratelyActive => 1.55m,
                ActivityLevel.VeryActive => 1.725m,
                ActivityLevel.ExtraActive => 1.9m,
                _ => 1.2m
            };

            decimal amr = bmr * activityMultiplier;

            // 3. Calculate Daily Target based on Goal
            profile.Daily_Calories = request.GoalType switch
            {
                GoalType.LoseWeight => amr - 500,
                GoalType.GainWeight => amr + 500,
                _ => amr // Maintain
            };

            _unitOfWork.GetRepository<int, UserProfile>().Update(profile);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
