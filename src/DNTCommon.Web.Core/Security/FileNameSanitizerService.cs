using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// SafeFile Download Service
    /// </summary>
    public class FileNameSanitizerService : IFileNameSanitizerService
    {
        private readonly ILogger<FileNameSanitizerService> _logger;

        /// <summary>
        /// SafeFile Download Service
        /// </summary>
        public FileNameSanitizerService(ILogger<FileNameSanitizerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Determines whether the requested file is safe to download.
        /// </summary>
        public SafeFile IsSafeToDownload(string folderPath, string requestedFileName)
        {
            if (string.IsNullOrWhiteSpace(requestedFileName))
            {
                return new SafeFile();
            }

            var fileName = Path.GetFileName(requestedFileName);
            if (!string.Equals(fileName, requestedFileName, StringComparison.Ordinal))
            {
                _logger.LogWarning($"Bad file request. Sanitized file name is different than the actual name. `{fileName}` != `{requestedFileName}`");
                return new SafeFile();
            }

            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Requested file not found: `{filePath}`");
                return new SafeFile();
            }

            if (isOutsideOfRootPath(filePath, folderPath))
            {
                _logger.LogWarning($"Bad file request. The requested file path `{filePath}` is outside of the root path `{folderPath}`.");
                return new SafeFile();
            }

            return new SafeFile { IsSafeToDownload = true, SafeFileName = fileName, SafeFilePath = filePath };
        }

        private static bool isOutsideOfRootPath(string filePath, string folder)
        {
            return !filePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase);
        }
    }
}