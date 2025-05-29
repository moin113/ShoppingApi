using Microsoft.AspNetCore.Hosting;                 // Provides IWebHostEnvironment
using Microsoft.AspNetCore.Http;                    // Provides IFormFile
using System;                                       // Provides Guid
using System.IO;                                    // Provides Path, Directory, FileStream
using System.Threading.Tasks;                       // Provides Task

namespace MyntraClone.API.Services.FileService
{
    public class LocalFileService : IFileService      // Implements your IFileService interface
    {
        private readonly IWebHostEnvironment _env;    // Holds environment info (ContentRoot, WebRoot)

        // Constructor: inject IWebHostEnvironment via DI
        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;                               // Save the injected environment
        }

        // Synchronous image save
        public string SaveImage(IFormFile imageFile)
        {
            // Determine the wwwroot path, fallback to current dir/wwwroot
            var webRoot = _env.WebRootPath
                          ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            // Combine to Images folder
            var uploadDir = Path.Combine(webRoot, "Images");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Generate a unique filename with original extension
            var uniqueFileName = Guid.NewGuid().ToString()
                                 + Path.GetExtension(imageFile.FileName);
            // Build the full disk path
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            // Open a file stream and write the uploaded contents
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            // Return the relative URL for client consumption
            return $"/Images/{uniqueFileName}";
        }

        // Asynchronous image save (non-blocking)
        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            // Determine the wwwroot path, fallback to current dir/wwwroot
            var webRoot = _env.WebRootPath
                          ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            // Combine to Images folder
            var uploadDir = Path.Combine(webRoot, "Images");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // Generate a unique filename with original extension
            var uniqueFileName = Guid.NewGuid().ToString()
                                 + Path.GetExtension(imageFile.FileName);
            // Build the full disk path
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            // Open a file stream configured for async I/O
            await using (var stream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,      // 80KB buffer
                useAsync: true))         // Enable async
            {
                // Asynchronously copy the contents
                await imageFile.CopyToAsync(stream);
            }

            // Return the relative URL for client consumption
            return $"/Images/{uniqueFileName}";
        }
    }
}
