using System.Text.RegularExpressions;
using DNTPersianUtils.Core;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Path Utils
/// </summary>
public static class PathUtils
{
    /// <summary>
    ///     '/'
    /// </summary>
    public const char MoveDirectorySeparator = '/';

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
    /// <param name="searchOption"></param>
    /// <param name="extensions"></param>
    /// <returns></returns>
    public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo,
        SearchOption searchOption,
        params string[] extensions)
    {
        ArgumentNullException.ThrowIfNull(dirInfo);
        var normalizedExtensions = extensions.Select(ext => ext.TrimStart(trimChar: '*'));

        return dirInfo.EnumerateFiles(searchPattern: "*", searchOption)
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
    /// <param name="searchOption"></param>
    /// <param name="extensions"></param>
    public static void DeleteFiles(this string path, SearchOption searchOption, params string[] extensions)
    {
        foreach (var file in new DirectoryInfo(path).GetFilesByExtensions(searchOption, extensions))
        {
            file.Delete();
        }
    }

    /// <summary>
    ///     Specifies that the operating system should open an existing file. When the file is opened, it should be truncated
    ///     so that its size is zero bytes.
    /// </summary>
    /// <param name="filePath"></param>
    public static void CleanFileContent(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
        fs.Close();
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

    /// <summary>
    ///     Creates a temporary file path with the given extension and returns its full path.
    /// </summary>
    public static string GetTempFilePath(this string? extension)
    {
        var path = Path.GetTempPath();
        var ext = extension.IsEmpty() ? ".tmp" : $".{extension.TrimStart(trimChar: '.')}";
        var fileName = $"{Guid.NewGuid():N}{ext}";

        return Path.Combine(path, fileName);
    }

    /// <summary>
    ///     Creates a temporary file with the given extension and returns its full path.
    /// </summary>
    public static string CreateTempFile(this string extension, byte[] contentBytes)
    {
        var tempFilePath = GetTempFilePath(extension);
        File.WriteAllBytes(tempFilePath, contentBytes);

        return tempFilePath;
    }

    /// <summary>
    ///     Creates a temporary file with the given extension and returns its full path.
    /// </summary>
    public static string CreateTempFile(this string extension, string content)
    {
        var tempFilePath = GetTempFilePath(extension);
        File.WriteAllText(tempFilePath, content);

        return tempFilePath;
    }

    /// <summary>
    ///     Checks the System.Security.Permissions.FileIOPermissionAccess.Append permission
    /// </summary>
    /// <param name="filePath">The file to open</param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static bool IsReadableFile(this string filePath, ILogger? logger = null)
    {
        try
        {
            using var _ = File.Open(filePath, FileMode.Append, FileAccess.Read);

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "IsReadablePath(`{Path}`)", filePath);

            return false;
        }
    }

    /// <summary>
    ///     Checks the System.Security.Permissions.FileIOPermissionAccess.Append permission
    /// </summary>
    /// <param name="filePath">The file to open</param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static bool IsWritableFile(this string filePath, ILogger? logger = null)
    {
        try
        {
            using var _ = File.Open(filePath, FileMode.Append, FileAccess.Write);

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "IsWritablePath(`{Path}`)", filePath);

            return false;
        }
    }

    /// <summary>
    ///     Returns the extension (including the period ".") of the specified path string.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetExtension(this string filePath) => Path.GetExtension(filePath);

    /// <summary>
    ///     Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileName(this string filePath) => Path.GetFileName(filePath);

    /// <summary>
    ///     Returns the directory information for the specified path.
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns>
    ///     Directory information for path, or null if path denotes a root directory or is null. Returns Empty if path
    ///     does not contain directory information.
    /// </returns>
    public static string? GetDirectoryName(this string dirPath) => Path.GetDirectoryName(dirPath);

    /// <summary>
    ///     Copies an existing dir to a new dir. Creates the destDirPath if it doesn't exist.
    /// </summary>
    /// <param name="sourceDirPath"></param>
    /// <param name="destDirPath"></param>
    /// <param name="copySubDirectories"></param>
    /// <param name="forceOverWrite"></param>
    /// <returns></returns>
    public static bool CopyDirectory(this string sourceDirPath,
        string destDirPath,
        bool copySubDirectories = true,
        bool forceOverWrite = true)
    {
        var sourceDirInfo = new DirectoryInfo(sourceDirPath);

        if (!sourceDirInfo.Exists)
        {
            return false;
        }

        var sourceDirs = sourceDirInfo.GetDirectories();

        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }

        var sourceFiles = sourceDirInfo.GetFiles();

        foreach (var sourceFile in sourceFiles)
        {
            var newDir = Path.Combine(destDirPath, sourceFile.Name);
            sourceFile.CopyTo(newDir, forceOverWrite);
        }

        if (copySubDirectories)
        {
            foreach (var sourceDir in sourceDirs)
            {
                var newDir = Path.Combine(destDirPath, sourceDir.Name);

                if (!CopyDirectory(sourceDir.FullName, newDir, copySubDirectories, forceOverWrite))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    ///     Return file(s) size in byte
    /// </summary>
    /// <param name="pathList"></param>
    /// <returns></returns>
    public static long GetFilesSize(params ICollection<string> pathList)
        => pathList.Where(File.Exists).Sum(path => new FileInfo(path).Length);

    /// <summary>
    ///     Return directory files size in byte
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="includeSubDirectories"></param>
    /// <returns></returns>
    public static long GetDirectorySize(this string path, string searchPattern, bool includeSubDirectories)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        var files = Directory.GetFiles(path, searchPattern,
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        return GetFilesSize(files);
    }

    /// <summary>
    ///     Copies an existing file to a new file. Overwriting a file of the same name is allowed.
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destFilePath"></param>
    /// <param name="forceOverWrite"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static bool TryCopyFileTo(this string sourceFilePath,
        string destFilePath,
        bool forceOverWrite,
        ILogger? logger = null)
    {
        try
        {
            File.Copy(sourceFilePath, destFilePath, forceOverWrite);

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "TryCopyFileTo(`{Path}`,`{To}`)", sourceFilePath, destFilePath);

            return false;
        }
    }

    /// <summary>
    ///     Test if user has write access to a folder
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static bool IsWritableDirectory(this string? dirPath, ILogger? logger = null)
    {
        try
        {
            if (dirPath.IsEmpty())
            {
                return false;
            }

            using var _ = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), bufferSize: 1,
                FileOptions.DeleteOnClose);

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "IsWritableDirectory(`{Path}``)", dirPath);

            return false;
        }
    }

    /// <summary>
    ///     Splits the path based on the current Path.DirectorySeparatorChar
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IReadOnlyList<string>? SplitPath(this string? path)
    {
        path = NormalizePath(path);

        return path?.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    /// <summary>
    ///     Normalizes Path.DirectorySeparatorChar in the given path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string? NormalizePath(this string? path)
        => string.IsNullOrEmpty(path)
            ? path
            : Path.DirectorySeparatorChar switch
            {
                '\\' => path.Replace(oldValue: "/", newValue: "\\", StringComparison.OrdinalIgnoreCase)
                    .TrimEnd(trimChar: '\\'),
                '/' => path.Replace(oldValue: "\\", newValue: "/", StringComparison.OrdinalIgnoreCase)
                    .TrimEnd(trimChar: '/'),
                _ => path.Replace(oldValue: "\\", newValue: "/", StringComparison.OrdinalIgnoreCase)
                    .TrimEnd(MoveDirectorySeparator)
            };
}