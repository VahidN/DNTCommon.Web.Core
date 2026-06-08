using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public static class LinuxZipSplitter
{
    /// <summary>
    ///     This is a Linux only solution. Use `ZipArchiveExtensions` and `SplitFileToMultiplePartsAsync` for the other
    ///     operating systems.
    /// </summary>
    public static async Task<IList<string>?> LinuxZipAndSplitFileAsync(this string? filePath,
        int partSizeMB,
        string? password,
        string? outputDirectory,
        string? outputFileName,
        bool overwriteExistingFiles,
        ZipCompressionLevel compressionLevel = ZipCompressionLevel.Maximum,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (!await IsZipInstalledAsync(cancellationToken))
        {
            throw new InvalidOperationException(message: """
                                                         You should install the `zip` application first:
                                                         sudo apt update && sudo apt install zip    # برای Debian/Ubuntu
                                                         sudo dnf install zip                       # برای Fedora/RHEL
                                                         """);
        }

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
        var (existingFiles, outputZipPath) =
            GetExistingZipFiles(isFile: true, filePath, outputDirectory, outputFileName);

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
                if (logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    logger.LogDebug(message: "The zip file already exists.");
                }

                return null;
            }
        }

        ICollection<string> argumentsList = password.IsEmpty() ? [] : ["-P", password];

        argumentsList.AddRange([
            string.Create(CultureInfo.InvariantCulture, $"-{(int)compressionLevel}"), "-s",
            string.Create(CultureInfo.InvariantCulture, $"{partSizeMB}m"), "-j", "-q", outputZipPath, filePath
        ]);

        var processInfo = await new ApplicationStartInfo
        {
            ProcessName = "zip",
            AppPath = "zip",
            ArgumentsList = argumentsList,
            WaitForExit = TimeSpan.FromMinutes(value: 2),
            KillProcessOnStart = false
        }.ExecuteProcessAsync(cancellationToken);

        if (processInfo.ExitCode != 0)
        {
            if (logger?.IsEnabled(LogLevel.Error) == true)
            {
                logger.LogError(message: "`Error processing zip file: {Error}", processInfo.ProcessOutput);
            }

            return null;
        }

        var (createdFiles, _) = GetExistingZipFiles(isFile: true, filePath, outputDirectory, outputFileName);

        if (logger?.IsEnabled(LogLevel.Debug) == true)
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
        string outputDirectory,
        string? outputFileName)
    {
        string name;

        if (outputFileName?.IsEmpty() == false)
        {
            name = outputFileName.GetFileNameWithoutExtension()!;
        }
        else if (isFile)
        {
            name = path.GetFileNameWithoutExtension()!;
        }
        else
        {
            name = new DirectoryInfo(path).Name;
        }

        var outputZipName = $"{name}.zip";
        var outputZipPath = outputDirectory.SafePathCombine(outputZipName)!;
        var outputBase = outputDirectory.SafePathCombine(name)!;

        return (
            Directory.GetFiles(outputDirectory, $"{name}.*")
                .Where(filePath => string.Equals(filePath, outputZipPath, StringComparison.OrdinalIgnoreCase) ||
                                   filePath.StartsWith($"{outputBase}.z", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.Ordinal)
                .ToList(), outputZipPath);
    }

    /// <summary>
    ///     This is a Linux only solution. Use `ZipArchiveExtensions` and `SplitFileToMultiplePartsAsync` for the other
    ///     operating systems.
    /// </summary>
    public static async Task<IList<string>?> LinuxZipAndSplitFolderAsync(this string? folderPath,
        int partSizeMB,
        string? password,
        string? outputDirectory,
        string? outputFileName,
        bool overwriteExistingFiles,
        ZipCompressionLevel compressionLevel = ZipCompressionLevel.Maximum,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (!await IsZipInstalledAsync(cancellationToken))
        {
            throw new InvalidOperationException(message: """
                                                         You should install the `zip` application first:
                                                         sudo apt update && sudo apt install zip    # For Debian/Ubuntu
                                                         sudo dnf install zip                       # For Fedora/RHEL
                                                         """);
        }

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
        var (existingFiles, outputZipPath) =
            GetExistingZipFiles(isFile: false, folderPath, outputDirectory, outputFileName);

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
                if (logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    logger.LogDebug(message: "The zip file already exists.");
                }

                return null;
            }
        }

        var parentDir = Directory.GetParent(folderPath)?.FullName;
        var folderNameOnly = new DirectoryInfo(folderPath).Name;

        ICollection<string> argumentsList = password.IsEmpty() ? [] : ["-P", password];

        argumentsList.AddRange([
            string.Create(CultureInfo.InvariantCulture, $"-{(int)compressionLevel}"), "-r", "-s",
            string.Create(CultureInfo.InvariantCulture, $"{partSizeMB}m"), "-q", outputZipPath, folderNameOnly
        ]);

        var processInfo = await new ApplicationStartInfo
        {
            ProcessName = "zip",
            AppPath = "zip",
            ArgumentsList = argumentsList,
            WaitForExit = TimeSpan.FromMinutes(value: 5),
            KillProcessOnStart = false,
            WorkingDirectory = parentDir
        }.ExecuteProcessAsync(cancellationToken);

        if (processInfo.ExitCode != 0)
        {
            if (logger?.IsEnabled(LogLevel.Error) == true)
            {
                logger.LogError(message: "`Error processing zip file: {Error}", processInfo.ProcessOutput);
            }

            return null;
        }

        var (createdFiles, _) = GetExistingZipFiles(isFile: false, folderPath, outputDirectory, outputFileName);

        if (logger?.IsEnabled(LogLevel.Debug) == true)
        {
            foreach (var createdFile in createdFiles)
            {
                logger.LogDebug(message: "Created zip file: {File}, Size: {Size}", createdFile,
                    new FileInfo(createdFile).Length.ToFormattedFileSize());
            }
        }

        return createdFiles.Count == 0 ? null : createdFiles;
    }

    public static async Task<bool> IsZipInstalledAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var processInfo = await new ApplicationStartInfo
            {
                ProcessName = "zip",
                AppPath = "zip",
                ArgumentsList = ["--version"],
                WaitForExit = TimeSpan.FromSeconds(value: 15),
                KillProcessOnStart = false
            }.ExecuteProcessAsync(cancellationToken);

            return processInfo.ExitCode == 0;
        }
        catch
        {
            // فایل zip پیدا نشد یا اجرا نمی‌شود
            return false;
        }
    }
}
