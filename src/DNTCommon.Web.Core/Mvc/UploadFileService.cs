using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Upload File Service Extensions
    /// </summary>
    public static class UploadFileServiceExtensions
    {
        /// <summary>
        /// Adds IUploadFileService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddUploadFileService(this IServiceCollection services)
        {
            services.AddSingleton<IUploadFileService, UploadFileService>();
            return services;
        }
    }

    /// <summary>
    /// Upload File Service
    /// </summary>
    public interface IUploadFileService
    {
        /// <summary>
        /// Saves the posted IFormFile to the specified directory asynchronously.
        /// </summary>
        /// <param name="formFile">The posted file.</param>
        /// <param name="destinationDirectoryName">A directory name in the wwwroot directory.</param>
        /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
        /// <returns></returns>
        Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile formFile, string destinationDirectoryName, bool allowOverwrite);

        /// <summary>
        /// Saves the posted IFormFile to a byte array.
        /// </summary>
        /// <param name="formFile">The posted file.</param>
        Task<byte[]> GetPostedFileDataAsync(IFormFile formFile);
    }

    /// <summary>
    /// Upload File Service
    /// </summary>
    public class UploadFileService : IUploadFileService
    {
        private const int MaxBufferSize = 0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

#if NETCOREAPP3_0 || NETCOREAPP3_1
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Upload File Service
        /// </summary>
        public UploadFileService(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
#else
        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// Upload File Service
        /// </summary>
        public UploadFileService(IHostingEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
#endif

        /// <summary>
        /// Saves the posted IFormFile to the specified directory asynchronously.
        /// </summary>
        /// <param name="formFile">The posted file.</param>
        /// <param name="destinationDirectoryName">A directory name in the wwwroot directory.</param>
        /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
        /// <returns></returns>
        public async Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile formFile, string destinationDirectoryName, bool allowOverwrite)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return (false, string.Empty);
            }

            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, destinationDirectoryName);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }

            var filePath = Path.Combine(uploadsRootFolder, formFile.FileName);
            if (File.Exists(filePath) && !allowOverwrite)
            {
                filePath = getUniqueFilePath(formFile, uploadsRootFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                                       FileShare.None,
                                       MaxBufferSize,
                                       // you have to explicitly open the FileStream as asynchronous
                                       // or else you're just doing synchronous operations on a background thread.
                                       useAsync: true))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return (true, filePath);
        }

        /// <summary>
        /// Saves the posted IFormFile to a byte array.
        /// </summary>
        /// <param name="formFile">The posted file.</param>
        public async Task<byte[]> GetPostedFileDataAsync(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private static string getUniqueFilePath(IFormFile formFile, string uploadsRootFolder)
        {
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            return Path.Combine(uploadsRootFolder, $"{fileName}.{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.{Guid.NewGuid().ToString("N")}{extension}");
        }
    }
}