using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Upload File Service Extensions
/// </summary>
public static class UploadFileServiceExtensions
{
    private const int
        MaxBufferSize =
            0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

    /// <summary>
    ///     Adds IUploadFileService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddUploadFileService(this IServiceCollection services)
    {
        services.AddSingleton<IUploadFileService, UploadFileService>();

        return services;
    }

    /// <summary>
    ///     Creates a unique file name if the file already exists.
    /// </summary>
    /// <param name="fileName">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <returns></returns>
    public static string GetUniqueFilePath(this string? fileName, string uploadsRootFolder)
    {
        if (fileName is null)
        {
            return string.Empty;
        }

        var filePath = Path.Combine(uploadsRootFolder, fileName);

        if (!File.Exists(filePath))
        {
            return filePath;
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);

        return Path.Combine(uploadsRootFolder,
            $"{fileNameWithoutExtension}.{DateTime.Now.ToString(format: "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture)}.{Guid.NewGuid():N}{extension}");
    }

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public static async Task<byte[]?> GetPostedFileDataAsync(this IFormFile? formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Saves the posted IFormFile to the specified directory asynchronously.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    /// <param name="uploadsRootFolder">The absolute path of the upload folder.</param>
    /// <param name="allowOverwrite">Creates a unique file name if the file already exists.</param>
    /// <returns></returns>
    public static async Task<(bool IsSaved, string SavedFilePath)> SavePostedFileAsync(this IFormFile? formFile,
        string uploadsRootFolder,
        bool allowOverwrite)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return (false, string.Empty);
        }

        uploadsRootFolder.CreateSafeDir();

        var filePath = Path.Combine(uploadsRootFolder, formFile.FileName);

        if (File.Exists(filePath) && !allowOverwrite)
        {
            filePath = GetUniqueFilePath(formFile.FileName, uploadsRootFolder) ??
                       throw new InvalidOperationException(message: "Posted file is null");
        }

        // you have to explicitly open the FileStream as asynchronous
        // or else you're just doing synchronous operations on a background thread.
        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
            MaxBufferSize, useAsync: true);

        await formFile.CopyToAsync(fileStream);

        return (true, filePath);
    }
}