using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Services.Abstraction.AttachmentService
{
    public interface IAttachmentService
    {
        Task<string?> Upload(string folderName, IFormFile file);
        bool Delete(string folderName, string fileName);
    }
}
