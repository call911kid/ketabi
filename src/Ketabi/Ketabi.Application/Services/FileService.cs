using Microsoft.AspNetCore.Http;

namespace Ketabi.Application.Services;


/// <summary>
/// Deprecated: This service is no longer used. File handling is now managed by the FileService in the Ketabi.Web project,
/// which has access to the IWebHostEnvironment for better path management and separation of concerns.
/// </summary>
public static class FileService
{
    private static readonly string _uploadPath;

    static FileService()
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public static async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided.");

        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("File size exceeds 10 MB.");

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public static bool DeleteFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var cleanFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_uploadPath, cleanFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }
}
