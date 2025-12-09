using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Upload File Service
/// </summary>
/// <remarks>
///     Upload File Service
/// </remarks>
public class UploadFileService(IWebHostEnvironment environment) : IUploadFileService
{
    private readonly IWebHostEnvironment _environment =
        environment ?? throw new ArgumentNullException(nameof(environment));

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="destinationDirectoryNames">Directory names in the wwwroot directory.</param>
    /// <returns></returns>
    public async Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        bool allowOverwrite,
        ICollection<string>? destinationDirectoryNames = null,
        CancellationToken cancellationToken = default)
    {
        if (formFile is null || formFile.Length == 0)
        {
            return (false, string.Empty);
        }

        var uploadsRootFolder = _environment.WebRootPath;

        if (destinationDirectoryNames is not null)
        {
            foreach (var folder in destinationDirectoryNames)
            {
                uploadsRootFolder = uploadsRootFolder.SafePathCombine(folder);
            }
        }

        return await SavePostedFileAsync(formFile, uploadsRootFolder, allowOverwrite, cancellationToken);
    }

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        string uploadsRootFolder,
        bool allowOverwrite,
        CancellationToken cancellationToken = default)
        => formFile.SavePostedFileAsync(uploadsRootFolder, allowOverwrite, cancellationToken);

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="cancellationToken"></param>
    public Task<byte[]?> GetPostedFileDataAsync(IFormFile? formFile, CancellationToken cancellationToken = default)
        => formFile.GetPostedFileDataAsync(cancellationToken);

    /// <summary>
    ///     Creates a unique file name if the file already exists.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <returns></returns>
    public string? GetUniqueFilePath(IFormFile? formFile, string uploadsRootFolder)
        => formFile?.FileName.GetUniqueFilePath(uploadsRootFolder);
}
