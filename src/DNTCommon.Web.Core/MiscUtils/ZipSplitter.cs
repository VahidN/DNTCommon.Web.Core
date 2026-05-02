using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public static class ZipSplitter
{
    /// <summary>
    ///     This is a Linux only solution. Use `ZipArchiveExtensions` and `SplitFileToMultiplePartsAsync` for the other
    ///     operating systems.
    /// </summary>
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
        var (existingFiles, outputZipPath) = GetExistingZipFiles(isFile: true, filePath, outputDirectory);

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
                "-s", string.Create(CultureInfo.InvariantCulture, $"{partSizeMB}m"), "-j", "-q", outputZipPath,
                filePath
            ],
            WaitForExit = TimeSpan.FromMinutes(value: 2),
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

        var (createdFiles, _) = GetExistingZipFiles(isFile: true, filePath, outputDirectory);

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

    public static (List<string> Parts, string OutputZipPath) GetExistingZipFiles(bool isFile,
        string path,
        string outputDirectory)
    {
        var name = isFile ? Path.GetFileNameWithoutExtension(path) : new DirectoryInfo(path).Name;
        var outputZipName = $"{name}.zip";
        var outputZipPath = outputDirectory.SafePathCombine(outputZipName)!;
        var outputBase = outputDirectory.SafePathCombine(name)!;

        return (
            Directory.GetFiles(outputDirectory, $"{name}.*")
                .Where(f => string.Equals(f, outputZipPath, StringComparison.OrdinalIgnoreCase) ||
                            f.StartsWith($"{outputBase}.z", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.Ordinal)
                .ToList(), outputZipPath);
    }

    /// <summary>
    ///     This is a Linux only solution. Use `ZipArchiveExtensions` and `SplitFileToMultiplePartsAsync` for the other
    ///     operating systems.
    /// </summary>
    public static async Task<IList<string>?> ZipAndSplitFolderToMultiplePartsAsync(this string? folderPath,
        int partSizeMB,
        string? outputDirectory,
        bool overwriteExistingFiles,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logger);

        folderPath = folderPath.NormalizePath();

        if (!folderPath.DirectoryExists())
        {
            throw new InvalidOperationException($"Input dir: `{folderPath}` doesn't exist.");
        }

        outputDirectory = outputDirectory.NormalizePath();

        if (outputDirectory.IsEmpty())
        {
            throw new InvalidOperationException(message: "Output dir is not set");
        }

        outputDirectory.TryCreateDirectory();

        // پاکسازی فایل‌های قبلی با همان نام (برای جلوگیری از تداخل)
        var (existingFiles, outputZipPath) = GetExistingZipFiles(isFile: false, folderPath, outputDirectory);

        if (existingFiles.Count > 0)
        {
            if (overwriteExistingFiles)
            {
                foreach (var file in existingFiles)
                {
                    file.TryDeleteDirectory(logger);
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

        var parentDir = Directory.GetParent(folderPath)?.FullName;
        var folderNameOnly = new DirectoryInfo(folderPath).Name;

        var processInfo = await new ApplicationStartInfo
        {
            ProcessName = "zip",
            ArgumentsList =
            [
                "-r", "-s", string.Create(CultureInfo.InvariantCulture, $"{partSizeMB}m"), "-q", outputZipPath,
                folderNameOnly
            ],
            WaitForExit = TimeSpan.FromMinutes(value: 5),
            KillProcessOnStart = false,
            WorkingDirectory = parentDir
        }.ExecuteProcessAsync(cancellationToken);

        if (processInfo.ExitCode != 0)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(message: "`Error processing zip file: {Error}", processInfo.ProcessOutput);
            }

            return null;
        }

        var (createdFiles, _) = GetExistingZipFiles(isFile: false, folderPath, outputDirectory);

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
}
