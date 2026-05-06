using Microsoft.AspNetCore.Http;

namespace Ketabi.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        bool DeleteFile(string fileName, string folderName);
    }
}