using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Upload File Service Extensions
/// </summary>
public static class UploadFileServiceExtensions
{
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
            $"{fileNameWithoutExtension}.{DateTime.UtcNow.ToString(format: "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture)}.{Guid.NewGuid():N}{extension}");
    }

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public static async Task<byte[]?> GetPostedFileDataAsync(this IFormFile? formFile)
    {
        if (formFile.IsNullOrEmpty())
        {
            return null;
        }

        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public static byte[]? GetPostedFileData(this IFormFile? formFile)
    {
        if (formFile.IsNullOrEmpty())
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        formFile.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public static Task<byte[]?> ToByteArrayAsync(this IFormFile? formFile) => formFile.GetPostedFileDataAsync();

    /// <summary>
    ///     Saves the posted IFormFile to a byte array.
    /// </summary>
    /// <param name="formFile">The posted file.</param>
    public static byte[]? ToByteArray(this IFormFile? formFile) => formFile.GetPostedFileData();

    /// <summary>
    ///     Saves the posted IFormFiles to  byte arrays.
    /// </summary>
    /// <param name="fromFiles">The posted files.</param>
    public static IEnumerable<Task<byte[]?>> ToByteArraysAsync(this IFormFileCollection? fromFiles)
    {
        if (fromFiles.IsNullOrEmpty())
        {
            yield break;
        }

        foreach (var fromFile in fromFiles)
        {
            yield return fromFile.GetPostedFileDataAsync();
        }
    }

    /// <summary>
    ///     Saves the posted IFormFiles to byte arrays.
    /// </summary>
    /// <param name="fromFiles">The posted files.</param>
    public static IEnumerable<byte[]?> ToByteArrays(this IFormFileCollection? fromFiles)
    {
        if (fromFiles.IsNullOrEmpty())
        {
            yield break;
        }

        foreach (var fromFile in fromFiles)
        {
            yield return fromFile.GetPostedFileData();
        }
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
        if (formFile.IsNullOrEmpty())
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
        await using var fileStream = filePath.CreateAsyncFileStream(FileMode.Create, FileAccess.Write);
        await formFile.CopyToAsync(fileStream);

        return (true, filePath);
    }

    /// <summary>
    ///     Converts the posted IFormFiles to a stream.
    /// </summary>
    public static Stream? ToStream(this IFormFile? formFile)
    {
        if (formFile.IsNullOrEmpty())
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        formFile.CopyTo(memoryStream);
        memoryStream.Seek(offset: 0, SeekOrigin.Begin);

        return memoryStream;
    }

    /// <summary>
    ///     Converts the posted IFormFiles to a stream.
    /// </summary>
    public static async Task<Stream?> ToStreamAsync(this IFormFile? formFile)
    {
        if (formFile.IsNullOrEmpty())
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        memoryStream.Seek(offset: 0, SeekOrigin.Begin);

        return memoryStream;
    }

    /// <summary>
    ///     Checks the file's length to detect if it has a data or not.
    /// </summary>
    public static bool IsNullOrEmpty([NotNullWhen(returnValue: false)] this IFormFile? formFile)
        => formFile is null || formFile.Length == 0;

    /// <summary>
    ///     Checks the fromFiles's count to detect if it has a file or not.
    ///     Then it checks the file's length of each entry to see if there is any data attached to them.
    /// </summary>
    public static bool IsNullOrEmpty([NotNullWhen(returnValue: false)] this IFormFileCollection? fromFiles)
    {
        if (fromFiles is null || fromFiles.Count == 0)
        {
            return true;
        }

        var isEmpty = true;

        foreach (var fromFile in fromFiles)
        {
            if (!fromFile.IsNullOrEmpty())
            {
                isEmpty = false;
            }
        }

        return isEmpty;
    }
}
