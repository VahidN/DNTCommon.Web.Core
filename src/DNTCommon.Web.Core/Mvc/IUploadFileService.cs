using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Upload File Service
/// </summary>
public interface IUploadFileService
{
    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <param name="destinationDirectoryNames">Directory names in the wwwroot directory.</param>
    /// <returns></returns>
    Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        bool allowOverwrite,
        params string[] destinationDirectoryNames);

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <returns></returns>
    Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(IFormFile? formFile,
        string uploadsRootFolder,
        bool allowOverwrite);

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    Task<byte[]?> GetPostedFileDataAsync(IFormFile? formFile);

    /// <summary>
    ///     Creates a unique file name if the file already exists.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <returns></returns>
    string GetUniqueFilePath(IFormFile? formFile, string uploadsRootFolder);
}