using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public static class ZipAndSplitter
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(value: 2);

    public static async Task<IList<string>?> ZipAndSplitFileToMultiplePartsAsync(this string filePath,
        string outputDirectory,
        int? partSizeMB,
        string? outputFileName,
        bool overwriteExistingFiles = true,
        string? password = null,
        ZipCompressionLevel compressionLevel = ZipCompressionLevel.Maximum,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (partSizeMB.HasValue && await LinuxZipSplitter.IsZipInstalledAsync(cancellationToken))
        {
            return await filePath.LinuxZipAndSplitFileAsync(partSizeMB.Value, password, outputDirectory, outputFileName,
                overwriteExistingFiles, compressionLevel, logger, cancellationToken);
        }

        var name = outputFileName?.IsEmpty() == false
            ? outputFileName.GetFileNameWithoutExtension()
            : filePath.GetFileNameWithoutExtension();

        var dataBackupFileName = string.Create(CultureInfo.InvariantCulture, $"{name}.zip");

        var backupZipFilePath = outputDirectory.SafePathCombine(dataBackupFileName);

        backupZipFilePath.CompressFilesToZipFile(filePath);
        await Task.Delay(Delay, cancellationToken);

        if (!partSizeMB.HasValue)
        {
            return [backupZipFilePath];
        }

        var parts = await backupZipFilePath.SplitFileAsync(outputDirectory, partsInfo =>
        {
            var totalWidth = partsInfo.TotalParts.CountDigits();
            var number = partsInfo.PartNumber.ToStringPadLeft(totalWidth);

            return string.Create(CultureInfo.InvariantCulture, $"{dataBackupFileName}{number}.part");
        }, partSizeMB.Value.ToBytes(FileSizeUnit.Megabyte), cancellationToken);

        backupZipFilePath.TryDeleteFile(logger);

        return parts;
    }

    public static async Task<IList<string>?> ZipAndSplitFolderToMultiplePartsAsync(this string folderPath,
        string outputDirectory,
        int? partSizeMB,
        string? outputFileName,
        bool overwriteExistingFiles = true,
        string? password = null,
        ZipCompressionLevel compressionLevel = ZipCompressionLevel.Maximum,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (partSizeMB.HasValue && await LinuxZipSplitter.IsZipInstalledAsync(cancellationToken))
        {
            return await folderPath.LinuxZipAndSplitFolderAsync(partSizeMB.Value, password, outputDirectory,
                outputFileName, overwriteExistingFiles, compressionLevel, logger, cancellationToken);
        }

        var name = outputFileName?.IsEmpty() == false
            ? outputFileName.GetFileNameWithoutExtension()
            : new DirectoryInfo(folderPath).Name;

        var dataBackupFileName = string.Create(CultureInfo.InvariantCulture, $"{name}.zip");

        var outputZipFilePath = outputDirectory.SafePathCombine(dataBackupFileName);

        folderPath.CompressFolderToZipFile(outputZipFilePath);
        await Task.Delay(Delay, cancellationToken);

        if (!partSizeMB.HasValue)
        {
            return [outputZipFilePath];
        }

        var partPaths = await outputZipFilePath.SplitFileAsync(outputDirectory, partsInfo =>
        {
            var totalWidth = partsInfo.TotalParts.CountDigits();
            var number = partsInfo.PartNumber.ToStringPadLeft(totalWidth);

            return string.Create(CultureInfo.InvariantCulture, $"{dataBackupFileName}{number}.part");
        }, partSizeMB.Value.ToBytes(FileSizeUnit.Megabyte), cancellationToken);

        outputZipFilePath.TryDeleteFile(logger);

        return partPaths;
    }
}
