using FoodRecognitionApp.Shared.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.AttachmentService
{
    public interface IAttachmentService
    {
        Task<Result<string>> Upload(string folderName, IFormFile file, CancellationToken ct = default);
        Result Delete(string folderName, string fileName);
    }
}
