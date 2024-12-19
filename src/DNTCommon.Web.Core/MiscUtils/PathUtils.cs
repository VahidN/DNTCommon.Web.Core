using System.Text.RegularExpressions;
using DNTPersianUtils.Core;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Path Utils
/// </summary>
public static class PathUtils
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(value: 3);

    /// <summary>
    ///     Returns the file's size in KB/ MB format
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string? GetFormattedFileSize(this string? filePath)
        => filePath.FileExists() ? new FileInfo(filePath).Length.ToFormattedFileSize() : null;

    /// <summary>
    ///     Computes the hash value for the specified Stream object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>A string representation that is encoded with uppercase hex characters</returns>
    public static string GetContentsSHA256(this string filePath)
    {
        using var sha256 = SHA256.Create();

        return filePath.GetContentsHash(sha256);
    }

    /// <summary>
    ///     Computes the hash value for the specified Stream object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="hashAlgorithm">Such as `using var sha256 = SHA256.Create();`</param>
    /// <returns>A string representation that is encoded with uppercase hex characters</returns>
    public static string GetContentsHash(this string filePath, HashAlgorithm hashAlgorithm)
    {
        ArgumentNullException.ThrowIfNull(hashAlgorithm);

        using var stream = File.OpenRead(filePath);
        var checksum = hashAlgorithm.ComputeHash(stream);

        return Convert.ToHexString(checksum);
    }

    /// <summary>
    ///     Computes the hash value for the specified Stream object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>A string representation that is encoded with uppercase hex characters</returns>
    public static string GetContentsSHA1(this string filePath)
    {
        using var sha1 = SHA1.Create();

        return filePath.GetContentsHash(sha1);
    }

    /// <summary>
    ///     Determines whether the specified file exists.
    /// </summary>
    /// <param name="filePath"></param>
    public static bool FileExists([NotNullWhen(returnValue: true)] this string? filePath)
        => !filePath.IsEmpty() && File.Exists(filePath);

    /// <summary>
    ///     Tries to delete a file without throwing an exception
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="logger"></param>
    public static bool TryDeleteFile(this string? filePath, ILogger? logger = null)
    {
        try
        {
            if (!filePath.IsEmpty() && File.Exists(filePath))
            {
                File.Delete(filePath);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "Failed to delete `{Path}` file.", filePath);
        }

        return false;
    }

    /// <summary>
    ///     Find files by their extensions
    /// </summary>
    /// <param name="dirInfo"></param>
    /// <param name="extensions"></param>
    /// <returns></returns>
    public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
    {
        ArgumentNullException.ThrowIfNull(dirInfo);
        var normalizedExtensions = extensions.Select(ext => ext.TrimStart(trimChar: '*'));

        return dirInfo.EnumerateFiles()
            .Where(fileInfo => normalizedExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path"></param>
    public static void CheckDirExists(this string? path)
    {
        if (!path.IsEmpty() && !Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    ///     Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path"></param>
    public static void TryCreateDirectory(this string? path) => path.CheckDirExists();

    /// <summary>
    ///     Deletes the given files
    /// </summary>
    /// <param name="path"></param>
    /// <param name="extensions"></param>
    public static void DeleteFiles(this string path, params string[] extensions)
    {
        foreach (var file in new DirectoryInfo(path).GetFilesByExtensions(extensions))
        {
            file.Delete();
        }
    }

    /// <summary>
    ///     Removes InvalidFileNameChars and InvalidPathChars from the given input
    /// </summary>
    public static string RemoveIllegalCharactersFromFileName(this string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        var regexSearch = $"{new string(Path.GetInvalidFileNameChars())}{new string(Path.GetInvalidPathChars())}";

        var r = new Regex($"[{Regex.Escape(regexSearch)}]", RegexOptions.Compiled | RegexOptions.IgnoreCase,
            MatchTimeout);

        return r.Replace(fileName, replacement: ".").GetPostSlug()!;
    }
}