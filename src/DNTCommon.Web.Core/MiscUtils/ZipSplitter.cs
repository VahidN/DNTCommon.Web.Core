using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public static class ZipSplitter
{
    public static async Task<IList<string>?> ZipAndSplitFileToMultiplePartsAsync(this string? filePath,
        int partSizeMB,
        string? outputDirectory,
        bool overwriteExistingFiles,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logger);

        filePath = filePath.NormalizePath();

        if (!filePath.FileExists())
        {
            throw new InvalidOperationException($"Input file: `{filePath}` doesn't exist.");
        }

        outputDirectory = outputDirectory.NormalizePath();

        if (outputDirectory.IsEmpty())
        {
            throw new InvalidOperationException(message: "Output dir is not set");
        }

        outputDirectory.TryCreateDirectory();

        // پاکسازی فایل‌های قبلی با همان نام (برای جلوگیری از تداخل)
        var (existingFiles, outputZipPath) = GetExistingZipFiles(filePath, outputDirectory);

        if (existingFiles.Count > 0)
        {
            if (overwriteExistingFiles)
            {
                foreach (var file in existingFiles)
                {
                    file.TryDeleteFile(logger);
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug(message: "The zip file already exists.");
                }

                return null;
            }
        }

        var processInfo = await new ApplicationStartInfo
        {
            ProcessName = "zip",
            ArgumentsList =
            [
                "-s", string.Create(CultureInfo.InvariantCulture, $"{partSizeMB}m"), "-q", outputZipPath, filePath
            ],
            WaitForExit = TimeSpan.FromMinutes(minutes: 2),
            KillProcessOnStart = false
        }.ExecuteProcessAsync(cancellationToken);

        if (processInfo.ExitCode != 0)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(message: "`Error processing zip file: {Error}", processInfo.ProcessOutput);
            }

            return null;
        }

        var (createdFiles, _) = GetExistingZipFiles(filePath, outputDirectory);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            foreach (var createdFile in createdFiles)
            {
                logger.LogDebug(message: "Created zip file: {File}, Size: {Size}", createdFile,
                    new FileInfo(createdFile).Length.ToFormattedFileSize());
            }
        }

        return createdFiles.Count == 0 ? null : createdFiles;
    }

    public static (List<string> Parts, string OutputZipPath) GetExistingZipFiles(string filePath,
        string outputDirectory)
    {
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        var outputZipName = $"{fileNameWithoutExt}.zip";
        var outputZipPath = outputDirectory.SafePathCombine(outputZipName)!;
        var outputBase = outputDirectory.SafePathCombine(fileNameWithoutExt)!;

        return (
            Directory.GetFiles(outputDirectory, $"{fileNameWithoutExt}.*")
                .Where(f => string.Equals(f, outputZipPath, StringComparison.OrdinalIgnoreCase) ||
                            f.StartsWith($"{outputBase}.z", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.Ordinal)
                .ToList(), outputZipPath);
    }
}
