using Ketabi.Application.Common;
using Ketabi.Application.Interfaces;

namespace Ketabi.Web.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException(Messages.Validation.RequiredField);

        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException(Messages.Validation.ProfilePicSize);

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Folders.Uploads, folderName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public bool DeleteFile(string fileName, string folderName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        if (fileName.Equals(AppConstants.DefaultProfilePic, StringComparison.OrdinalIgnoreCase)) return true;

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, AppConstants.Folders.Uploads, folderName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }
}