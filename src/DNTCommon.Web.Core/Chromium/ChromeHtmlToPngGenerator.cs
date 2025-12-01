using System.Text;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public class ChromeHtmlToPngGenerator(
    IExecuteApplicationProcess executeApplicationProcess,
    ILockerService lockerService,
    ILogger<ChromeHtmlToPngGenerator> logger,
    IAntiXssService antiXssService) : IHtmlToPngGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    /// <summary>
    ///     High level method that converts HTML to PNG.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        using var locker =
            await lockerService.LockAsync<ExecuteApplicationProcess>(options.VirtualTimeBudget, cancellationToken);

        var arguments = CreateArguments(options);

        var appPath = options.ChromePath;

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
            KillProcessOnStart = options.KillProcessOnStart
        }, cancellationToken);

        antiXssService.GetSanitizedHtml($"{options}{Environment.NewLine}{log}").LogPossibleErrorsOrWarnings(logger);

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
            string.Create(CultureInfo.InvariantCulture,
                $"--virtual-time-budget={options.VirtualTimeBudgetTotalMilliseconds}"),
            string.Create(CultureInfo.InvariantCulture, $"--window-size=\"{options.Width},{options.Height}\"")
        ];

        var arguments = new StringBuilder();
        arguments.AppendJoin(separator: " ", parameters);

        if (string.IsNullOrEmpty(options.OutputFilePath))
        {
            throw new InvalidOperationException(message: "OutputFilePath is null");
        }

        if (string.IsNullOrEmpty(options.SourceHtmlFileOrUri))
        {
            throw new InvalidOperationException(message: "SourceHtmlFileOrUri is null");
        }

        arguments.Append(CultureInfo.InvariantCulture,
            $" --screenshot=\"{options.OutputFilePath}\" \"{options.SourceHtmlFileOrUri}\" ");

        return arguments.ToString();
    }

    private bool IsValidImageFile(HtmlToPngGeneratorOptions options)
    {
        if (options.OutputFilePath.IsValidImageFile(logger: logger))
        {
            return true;
        }

        options.OutputFilePath.TryDeleteFile(logger);
        logger.LogError(message: "`{File} was an invalid image file.`", options.OutputFilePath);

        return false;
    }

    private static async Task TryResizeFileAsync(HtmlToPngGeneratorOptions options)
    {
        var pngFile = options.OutputFilePath;

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
}
