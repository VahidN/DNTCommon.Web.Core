using System.Text;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PDF.
/// </summary>
public class ChromeHtmlToPdfGenerator(
    IExecuteApplicationProcess executeApplicationProcess,
    ILockerService lockerService,
    ILogger<ChromeHtmlToPdfGenerator> logger) : IHtmlToPdfGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    /// <summary>
    ///     High level method that converts HTML to PDF.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePdfFromHtmlAsync(HtmlToPdfGeneratorOptions options)
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

        options.OutputPdfFile.AddMetadataToPdfFile(options.DocumentMetadata);

        return log;
    }

    private static string CreateArguments(HtmlToPdfGeneratorOptions options)
    {
        string[] parameters =
        [
            ..ChromeGeneralParameters.GeneralParameters, "--no-pdf-header-footer",
            "--generate-pdf-document-outline", "--export-tagged-pdf"
        ];

        var arguments = new StringBuilder();
        arguments.AppendJoin(separator: " ", parameters);

        if (string.IsNullOrEmpty(options.OutputPdfFile))
        {
            throw new InvalidOperationException(message: "OutputPdfFile is null");
        }

        if (string.IsNullOrEmpty(options.SourceHtmlFileOrUri))
        {
            throw new InvalidOperationException(message: "SourceHtmlFileOrUri is null");
        }

        arguments.Append(CultureInfo.InvariantCulture,
            $" --print-to-pdf=\"{options.OutputPdfFile}\" \"{options.SourceHtmlFileOrUri}\" ");

        return arguments.ToString();
    }

    private static string? GetChromePath(HtmlToPdfGeneratorOptions options)
        => !string.IsNullOrWhiteSpace(options.ChromeExecutablePath)
            ? options.ChromeExecutablePath
            : ChromeFinder.Find();
}