using FoodRecognitionApp.Shared.Common;
using FoodRecognitionApp.Shared.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.Profile
{
    public interface IProfileService
    {
        Task<Result<CreateProfileResponse>> CreateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken ct = default);
        Task<Result<CreateProfileResponse>> UpdateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken ct = default);

    }
}
