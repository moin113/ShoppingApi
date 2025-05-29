using Microsoft.AspNetCore.Http;     // Provides IFormFile for accessing uploaded files
using System.Threading.Tasks;        // Provides Task<T> for async method signatures

namespace MyntraClone.API.Services.FileService
{
    public interface IFileService        // Defines the contract for file storage operations
    {
        // Synchronous save: writes the file and blocks until complete
        string SaveImage(IFormFile file);

        // Asynchronous save: returns a Task so the call can be awaited without blocking
        Task<string> SaveImageAsync(IFormFile file);
    }
}
