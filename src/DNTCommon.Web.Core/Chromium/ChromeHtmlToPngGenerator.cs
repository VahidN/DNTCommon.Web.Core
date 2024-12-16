using System.Text;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public class ChromeHtmlToPngGenerator(
    IExecuteApplicationProcess executeApplicationProcess,
    ILockerService lockerService,
    ILogger<ChromeHtmlToPngGenerator> logger) : IHtmlToPngGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    /// <summary>
    ///     High level method that converts HTML to PNG.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        using var locker = await lockerService.LockAsync<ExecuteApplicationProcess>();

        var arguments = CreateArguments(options);

        var appPath = GetChromePath(options);

        if (string.IsNullOrWhiteSpace(appPath))
        {
            throw new InvalidOperationException(ErrorMessage);
        }

        var log = await executeApplicationProcess.ExecuteProcessAsync(new ApplicationStartInfo
        {
            ProcessName = "chrome",
            Arguments = arguments,
            AppPath = appPath,
            WaitForExit = options.WaitForExit,
            KillProcessOnStart = true
        });

        log.LogPossibleErrorsOrWarnings(logger);

        if (!IsValidImageFile(options))
        {
            return log;
        }

        await TryResizeFileAsync(options);

        return log;
    }

    private static string CreateArguments(HtmlToPngGeneratorOptions options)
    {
        string[] parameters =
        [
            ..ChromeGeneralParameters.GeneralParameters,
            Invariant($"--window-size=\"{options.Width},{options.Height}\"")
        ];

        var arguments = new StringBuilder();
        arguments.AppendJoin(separator: " ", parameters);

        if (string.IsNullOrEmpty(options.OutputPngFile))
        {
            throw new InvalidOperationException(message: "OutputPngFile is null");
        }

        if (string.IsNullOrEmpty(options.SourceHtmlFileOrUri))
        {
            throw new InvalidOperationException(message: "SourceHtmlFileOrUri is null");
        }

        arguments.Append(CultureInfo.InvariantCulture,
            $" --screenshot=\"{options.OutputPngFile}\" \"{options.SourceHtmlFileOrUri}\" ");

        return arguments.ToString();
    }

    private bool IsValidImageFile(HtmlToPngGeneratorOptions options)
    {
        if (options.OutputPngFile.IsValidImageFile(logger: logger))
        {
            return true;
        }

        options.OutputPngFile.TryDeleteFile(logger);
        logger.LogError(message: "`{File} was an invalid image file.`", options.OutputPngFile);

        return false;
    }

    private static async Task TryResizeFileAsync(HtmlToPngGeneratorOptions options)
    {
        var pngFile = options.OutputPngFile;

        if (!pngFile.FileExists() || options is not { ResizeImageOptions: not null })
        {
            return;
        }

        var newData = pngFile.ResizeImage(options.ResizeImageOptions);

        if (newData is not null)
        {
            await File.WriteAllBytesAsync(pngFile, newData);
        }
    }

    private static string? GetChromePath(HtmlToPngGeneratorOptions options)
        => !string.IsNullOrWhiteSpace(options.ChromeExecutablePath)
            ? options.ChromeExecutablePath
            : ChromeFinder.Find();
}