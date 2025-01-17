using System.IO.Compression;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     ZipArchive Extensions
/// </summary>
public static class ZipArchiveExtensions
{
    /// <summary>
    ///     Creates an empty entry that has the specified entry name and compression level in the zip archive and then adds the
    ///     provided content to it.
    /// </summary>
    /// <param name="zipArchive">Represents a package of compressed files in the zip archive format.</param>
    /// <param name="entryName">
    ///     A path, relative to the root of the archive, that specifies the name of the entry to be
    ///     created.
    /// </param>
    /// <param name="data"></param>
    public static void AddToZipArchive(this ZipArchive zipArchive, string entryName, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(zipArchive);

        var zipEntry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);
        zipEntry.CopyToZipEntryStream(data);
    }

    /// <summary>
    ///     Creates an empty entry that has the specified entry name and compression level in the zip archive and then adds the
    ///     provided content to it.
    /// </summary>
    /// <param name="zipArchive">Represents a package of compressed files in the zip archive format.</param>
    /// <param name="entryName">
    ///     A path, relative to the root of the archive, that specifies the name of the entry to be
    ///     created.
    /// </param>
    /// <param name="data"></param>
    public static void AddToZipArchive(this ZipArchive zipArchive, string entryName, string data)
    {
        ArgumentNullException.ThrowIfNull(zipArchive);

        var zipEntry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);
        zipEntry.CopyToZipEntryStream(data);
    }

    /// <summary>
    ///     Archives a file by compressing it and adding it to the zip archive.
    /// </summary>
    /// <param name="zipArchive">Represents a package of compressed files in the zip archive format.</param>
    /// <param name="entryName">
    ///     A path, relative to the root of the archive, that specifies the name of the entry to be
    ///     created.
    /// </param>
    /// <param name="fileInfo">
    ///     The path to the file to be archived. You can specify either a relative or an absolute path. A
    ///     relative path is interpreted as relative to the current working directory.
    /// </param>
    public static void AddToZipArchive(this ZipArchive zipArchive, string entryName, FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(zipArchive);
        ArgumentNullException.ThrowIfNull(fileInfo);

        zipArchive.CreateEntryFromFile(fileInfo.FullName, entryName, CompressionLevel.Optimal);
    }

    /// <summary>
    ///     Replaces a compressed file within a zip archive with the given data.
    /// </summary>
    /// <param name="zipEntry">Represents a compressed file within a zip archive.</param>
    /// <param name="data"></param>
    public static void CopyToZipEntryStream(this ZipArchiveEntry zipEntry, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(zipEntry);

        using var originalFileStream = new MemoryStream(data);
        using var zipEntryStream = zipEntry.Open();
        originalFileStream.CopyTo(zipEntryStream);
    }

    /// <summary>
    ///     Replaces a compressed file within a zip archive with the given data.
    /// </summary>
    /// <param name="zipEntry">Represents a compressed file within a zip archive.</param>
    /// <param name="data"></param>
    public static void CopyToZipEntryStream(this ZipArchiveEntry zipEntry, string data)
    {
        ArgumentNullException.ThrowIfNull(zipEntry);

        CopyToZipEntryStream(zipEntry, Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    ///     Creates a zip archive that contains the files and directories from the specified directory, uses the specified
    ///     compression level, and optionally includes the base directory.
    /// </summary>
    /// <param name="folderPath">
    ///     The path to the directory to be archived, specified as a relative or absolute path. A relative
    ///     path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="outputZipFilePath">
    ///     The path of the archive to be created, specified as a relative or absolute path. A
    ///     relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="includeBaseDirectory">
    ///     true to include the directory name from sourceDirectoryName at the root of the
    ///     archive; false to include only the contents of the directory.
    /// </param>
    public static void CompressFolderToZipFile(this string folderPath,
        string outputZipFilePath,
        bool includeBaseDirectory = false)
        => ZipFile.CreateFromDirectory(folderPath, outputZipFilePath, CompressionLevel.Optimal, includeBaseDirectory,
            Encoding.UTF8);

    /// <summary>
    ///     Creates a zip archive that contains the specified files, uses the specified
    ///     compression level, and optionally includes the base directory.
    /// </summary>
    /// <param name="files">
    ///     The path to the files to be archived, specified as a relative or absolute path. A relative
    ///     path is interpreted as relative to the current working directory.
    /// </param>
    /// <param name="outputZipFilePath">
    ///     The path of the archive to be created, specified as a relative or absolute path. A
    ///     relative path is interpreted as relative to the current working directory.
    /// </param>
    public static void CompressFilesToZipFile(this string outputZipFilePath, params IList<string> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        if (File.Exists(outputZipFilePath))
        {
            File.Delete(outputZipFilePath);
        }

        using var zipArchive = ZipFile.Open(outputZipFilePath, ZipArchiveMode.Create, Encoding.UTF8);

        foreach (var file in files.Distinct())
        {
            var fileInfo = new FileInfo(file);

            if (fileInfo.Exists)
            {
                zipArchive.AddToZipArchive(fileInfo.Name, fileInfo);
            }
        }
    }

    /// <summary>
    ///     Extracts all the files in the specified archive to a directory on the file system.
    /// </summary>
    /// <param name="zipFilePath">The path on the file system to the archive that is to be extracted.</param>
    /// <param name="extractDir">The path to the destination directory on the file system.</param>
    /// <param name="overwriteFiles">true to overwrite files; false otherwise.</param>
    public static void DecompressZipFile(this string zipFilePath, string extractDir, bool overwriteFiles = true)
    {
        if (!Directory.Exists(extractDir))
        {
            Directory.CreateDirectory(extractDir);
        }

        ZipFile.ExtractToDirectory(zipFilePath, extractDir, Encoding.UTF8, overwriteFiles);
    }

    /// <summary>
    ///     Creates a gzip archive that contains the sourceFilePath.
    /// </summary>
    /// <param name="sourceFilePath">The path to the file to be archived</param>
    /// <param name="outputGZipFilePath">The path of the archive to be created</param>
    /// <param name="overwriteFile">true to overwrite files; false otherwise.</param>
    public static void CompressFileToGZipFile(this string sourceFilePath,
        string outputGZipFilePath,
        bool overwriteFile = true)
    {
        using var source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        using var destination = new FileStream(outputGZipFilePath, overwriteFile ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write, FileShare.None);

        using var gZipStream = new GZipStream(destination, CompressionMode.Compress);
        source.CopyTo(gZipStream);
    }

    /// <summary>
    ///     Extracts the specified gzip archive to a file on the file system.
    /// </summary>
    /// <param name="gZipFilePath">The path on the file system to the archive that is to be extracted.</param>
    /// <param name="destinationFilePath">The path to the destination file on the file system.</param>
    /// <param name="overwriteFile">true to overwrite files; false otherwise.</param>
    public static void DecompressGZipFile(this string gZipFilePath,
        string destinationFilePath,
        bool overwriteFile = true)
    {
        using var source = new FileStream(gZipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        using var destination = new FileStream(destinationFilePath,
            overwriteFile ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None);

        using var gZipStream = new GZipStream(source, CompressionMode.Decompress);
        gZipStream.CopyTo(destination);
    }
}