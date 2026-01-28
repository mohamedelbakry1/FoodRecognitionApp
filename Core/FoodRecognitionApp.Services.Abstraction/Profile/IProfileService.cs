using FoodRecognitionApp.Shared.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Profile
{
    public interface IProfileService
    {
        Task<CreateProfileResponse> CreateProfileAsync(int userId,CreateProfileRequest request);

        Task UpdateProfileAsync(int userId, CreateProfileRequest request);

    }
}
