using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Upload File Service
/// </summary>
public class UploadFileService : IUploadFileService
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    ///     Upload File Service
    /// </summary>
    public UploadFileService(IWebHostEnvironment environment)
        => _environment = environment ?? throw new ArgumentNullException(nameof(environment));

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <param name="destinationDirectoryNames">Directory names in the wwwroot directory.</param>
    /// <returns></returns>
    public async Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        bool allowOverwrite,
        params string[] destinationDirectoryNames)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return (false, string.Empty);
        }

        var uploadsRootFolder = Path.Combine(_environment.WebRootPath);

        if (destinationDirectoryNames is not null)
        {
            foreach (var folder in destinationDirectoryNames)
            {
                uploadsRootFolder = Path.Combine(uploadsRootFolder, folder);
            }
        }

        return await SavePostedFileAsync(formFile, uploadsRootFolder, allowOverwrite);
    }

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <returns></returns>
    public Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        string uploadsRootFolder,
        bool allowOverwrite)
        => formFile.SavePostedFileAsync(uploadsRootFolder, allowOverwrite);

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public Task<byte[]?> GetPostedFileDataAsync(IFormFile? formFile) => formFile.GetPostedFileDataAsync();

    /// <summary>
    ///     Creates a unique file name if the file already exists.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <returns></returns>
    public string? GetUniqueFilePath(IFormFile? formFile, string uploadsRootFolder)
        => formFile?.FileName.GetUniqueFilePath(uploadsRootFolder);
}