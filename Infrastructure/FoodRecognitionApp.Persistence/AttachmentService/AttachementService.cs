using FoodRecognitionApp.Services.Abstraction.AttachmentService;
using FoodRecognitionApp.Shared.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Persistence.AttachmentService
{
    internal class AttachmentService(
        IWebHostEnvironment _webHost,
        ILogger<AttachmentService> _logger) : IAttachmentService
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];
        private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/pjpeg"];
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly Dictionary<string, List<byte[]>> FileSignatures = new()
        {
            { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".jpg",  new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".png",  new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } }
        };
        public async Task<Result<string>> Upload(string folderName, IFormFile file, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return Error.Validation("Attachment.InvalidFolder", "Folder name cannot be empty.");
            if (file is null || file.Length == 0)
                return Error.Validation("Attachment.EmptyFile", "File cannot be null or empty.");
            if (file.Length > MaxFileSize)
                return Error.Validation("Attachment.FileTooLarge", "File size exceeds the allowed limit (5 MB).");
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return Error.Validation("Attachment.InvalidExtension", "File extension is not allowed. Only .jpg, .jpeg, and .png are permitted.");
            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return Error.Validation("Attachment.InvalidMimeType", "Invalid content type.");
            if (!IsValidFileSignature(file, extension))
                return Error.Validation("Attachment.FakeImage", "File content does not match a valid image signature.");
            try
            {
                var basePath = _webHost.WebRootPath ?? _webHost.ContentRootPath;
                var folderPath = Path.Combine(basePath, folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream, ct);
                }
                _logger.LogInformation("File {FileName} uploaded successfully to {Folder}.", fileName, folderName);

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to folder {Folder}.", folderName);
                return Error.Failure("Attachment.UploadFailed", $"Failed to upload file: {ex.Message}");
            }
        }
        public Result Delete(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                return Error.Validation("Attachment.EmptyFile", "File cannot be null or empty."); ;
            try
            {
                var safeFileName = Path.GetFileName(fileName);
                var basePath = _webHost.WebRootPath ?? _webHost.ContentRootPath;
                var folderPath = Path.Combine(basePath, folderName);
                var fullPath = Path.Combine(folderPath, safeFileName);
                var canonicalFolderPath = Path.GetFullPath(folderPath);
                var canonicalFullPath = Path.GetFullPath(fullPath);
                if (!canonicalFullPath.StartsWith(canonicalFolderPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Security Alert: Path traversal attempt: {File}", fileName);
                    return Error.Forbidden("Attachment.PathTraversal", "Invalid file path detected.");
                }
                if (!File.Exists(canonicalFullPath))
                    return Error.NotFound("Attachment.FileNotFound", "The specified file was not found.");

                File.Delete(canonicalFullPath);
                _logger.LogInformation("File {FileName} deleted from {Folder}.", safeFileName, folderName);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {FileName} from {Folder}.", fileName, folderName);
                return Error.Failure("Attachment.DeleteFailed", $"Failed to delete file: {ex.Message}");
            }
        }
        private static bool IsValidFileSignature(IFormFile file, string extension)
        {
            if (!FileSignatures.TryGetValue(extension, out var signatures))
                return false;
            using var reader = new BinaryReader(file.OpenReadStream());
            var maxSignatureLength = signatures.Max(s => s.Length);
            var headerBytes = reader.ReadBytes(maxSignatureLength);
            return signatures.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig));
        }
    }
}
