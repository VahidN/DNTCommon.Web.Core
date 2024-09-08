using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     SafeFile Download Service
/// </summary>
/// <remarks>
///     SafeFile Download Service
/// </remarks>
public class FileNameSanitizerService(ILogger<FileNameSanitizerService> logger, IAntiXssService antiXssService)
    : IFileNameSanitizerService
{
    private readonly ILogger<FileNameSanitizerService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Determines whether the requested file is safe to download.
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
            _logger.LogWarning(
                message:
                "Bad file request. Sanitized file name is different than the actual name. `{FileName}` != `{RequestedFileName}`",
                antiXssService.GetSanitizedHtml(fileName), antiXssService.GetSanitizedHtml(requestedFileName));

            return new SafeFile();
        }

        var filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning(message: "Requested file not found: `{FilePath}`",
                antiXssService.GetSanitizedHtml(filePath));

            return new SafeFile();
        }

        if (IsOutsideOfRootPath(filePath, folderPath))
        {
            _logger.LogWarning(
                message:
                "Bad file request. The requested file path `{FilePath}` is outside of the root path `{FolderPath}`.",
                antiXssService.GetSanitizedHtml(filePath), antiXssService.GetSanitizedHtml(folderPath));

            return new SafeFile();
        }

        return new SafeFile
        {
            IsSafeToDownload = true,
            SafeFileName = fileName,
            SafeFilePath = filePath
        };
    }

    private static bool IsOutsideOfRootPath(string filePath, string folder)
        => !filePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase);
}