using FoodRecognitionApp.Domain.Contracts;
using FoodRecognitionApp.Domain.Entities;
using FoodRecognitionApp.Domain.Entities.Enums;
using FoodRecognitionApp.Services.Abstraction.Profile;
using FoodRecognitionApp.Services.Specifications;
using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Profile;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Profile
{
    internal class ProfileService(IUnitOfWork _unitOfWork, ILogger<ProfileService> _logger) : IProfileService
    {
        public async Task<Result<CreateProfileResponse>> CreateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken ct = default)
        {
            var existSpec = new ProfileByUserIdSpecification(userId);
            var existingProfile = await _unitOfWork.GetRepository<int, UserProfile>().GetById(existSpec, ct: ct);
            if (existingProfile is not null)
            {
                _logger.LogWarning($"Attempt to create duplicate profile for UserId {userId}");
                return Error.Conflict("Profile.AlreadyExists","A profile already exists for this user. Use the update endpoint.");
            }

            var (dailyCalories, amr, bmr) = CalculateDailyCalories(request);

            var profile = new UserProfile
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
            _unitOfWork.GetRepository<int, UserProfile>().Add(profile);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result <= 0)
            {
                _logger.LogError($"Failed to create profile for UserId {userId}");
                return Error.Failure("Profile.CreateFailed","Could not create profile, please try again later.");
            }

            _logger.LogInformation($"Profile created for UserId {userId}");

            return new CreateProfileResponse
            {
                BMR = bmr,
                AMR = amr,
                DailyCaloriesTarget = dailyCalories
            };
        }

        public async Task<Result<CreateProfileResponse>> UpdateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken ct = default)
        {
            var spec = new ProfileByUserIdSpecification(userId);
            var profile = await _unitOfWork.GetRepository<int, UserProfile>().GetById(spec, true, ct);
            if (profile is null)
                return Error.NotFound("Profile.NotFound",$"Profile for user {userId} was not found.");

            var data = CalculateDailyCalories(request);

            profile.Age = request.Age;
            profile.Weight = request.Weight;
            profile.Height = request.Height;
            profile.Gender = request.Gender;
            profile.ActivityLevel = request.ActivityLevel;
            profile.GoalType = request.GoalType;
            profile.Daily_Calories = data.dailyCalories;

            _unitOfWork.GetRepository<int, UserProfile>().Update(profile);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result <= 0)
            {
                _logger.LogError($"UpdateProfile – SaveChangesAsync returned {result} for UserId {userId}");
                return Error.Failure("Profile.UpdateFailed","Could not persist the profile changes. Please try again later.");
            }

            _logger.LogInformation($"Profile updated for UserId {userId}");

            return new CreateProfileResponse
            {
                BMR = data.bmr,
                AMR = data.amr,
                DailyCaloriesTarget = data.dailyCalories
            };
        }



        private static (decimal dailyCalories, decimal amr, decimal bmr) CalculateDailyCalories(CreateProfileRequest request)
        {
            decimal bmr = request.Gender == Gender.Male
                ? (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) + 5
                : (10 * request.Weight) + (6.25m * request.Height) - (5 * request.Age) - 161;
            decimal activityMultiplier = request.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2m,
                ActivityLevel.LightlyActive => 1.375m,
                ActivityLevel.ModeratelyActive => 1.55m,
                ActivityLevel.HighlyActive => 1.725m,
                _ => 1.2m
            };
            decimal amr = bmr * activityMultiplier;
            decimal dailyCalories = request.GoalType switch
            {
                GoalType.LoseWeight => amr - 500,
                GoalType.GainWeight => amr + 500,
                _ => amr
            };
            return (dailyCalories, amr, bmr);
        }
    }
}
