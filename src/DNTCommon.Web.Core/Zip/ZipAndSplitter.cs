using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public static class ZipAndSplitter
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(value: 2);

    public static async Task<IList<string>?> ZipAndSplitFileToMultiplePartsAsync(this string filePath,
        string outputDirectory,
        int partSizeMB,
        bool appendSecureGuidToOutputName = true,
        bool overwriteExistingFiles = true,
        string? password = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (await LinuxZipSplitter.IsZipInstalledAsync(cancellationToken))
        {
            return await filePath.LinuxZipAndSplitFileAsync(partSizeMB, password, outputDirectory,
                overwriteExistingFiles, logger, cancellationToken);
        }

        var salt = appendSecureGuidToOutputName
            ? string.Create(CultureInfo.InvariantCulture,
                $".{DateTime.UtcNow:yyyyMMdd_HHmmss}.{Guid.CryptographicallySecureGuid:N}")
            : "";

        var dataBackupFileName = string.Create(CultureInfo.InvariantCulture, $"{filePath.GetFileName()}{salt}.zip");
        var backupZipFilePath = outputDirectory.SafePathCombine(dataBackupFileName)!;

        backupZipFilePath.CompressFilesToZipFile(filePath);
        await Task.Delay(Delay, cancellationToken);

        var parts = await backupZipFilePath.SplitFileAsync(outputDirectory, partsInfo =>
        {
            var totalWidth = partsInfo.TotalParts.CountDigits();
            var number = partsInfo.PartNumber.ToStringPadLeft(totalWidth);

            return string.Create(CultureInfo.InvariantCulture, $"{dataBackupFileName}_{number}.part");
        }, partSizeMB.ToBytes(FileSizeUnit.Megabyte), cancellationToken);

        backupZipFilePath.TryDeleteFile(logger);

        return parts;
    }

    public static async Task<IList<string>?> ZipAndSplitFolderToMultiplePartsAsync(this string folderPath,
        string outputDirectory,
        int partSizeMB,
        bool appendSecureGuidToOutputName = true,
        bool overwriteExistingFiles = true,
        string? password = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (await LinuxZipSplitter.IsZipInstalledAsync(cancellationToken))
        {
            return await folderPath.LinuxZipAndSplitFolderAsync(partSizeMB, password, outputDirectory,
                overwriteExistingFiles, logger, cancellationToken);
        }

        var salt = appendSecureGuidToOutputName
            ? string.Create(CultureInfo.InvariantCulture,
                $".{DateTime.UtcNow:yyyyMMdd_HHmmss}.{Guid.CryptographicallySecureGuid:N}")
            : "";

        var dataBackupFileName = string.Create(CultureInfo.InvariantCulture,
            $"{new DirectoryInfo(folderPath).Name}{salt}.zip");

        var outputZipFilePath = outputDirectory.SafePathCombine(dataBackupFileName)!;

        folderPath.CompressFolderToZipFile(outputZipFilePath);
        await Task.Delay(Delay, cancellationToken);

        var partPaths = await outputZipFilePath.SplitFileAsync(outputDirectory, partsInfo =>
        {
            var totalWidth = partsInfo.TotalParts.CountDigits();
            var number = partsInfo.PartNumber.ToStringPadLeft(totalWidth);

            return string.Create(CultureInfo.InvariantCulture, $"{dataBackupFileName}_{number}.part");
        }, partSizeMB.ToBytes(FileSizeUnit.Megabyte), cancellationToken);

        outputZipFilePath.TryDeleteFile(logger);

        return partPaths;
    }
}
