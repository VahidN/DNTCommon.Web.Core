using System.Text;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PDF.
/// </summary>
public class ChromeHtmlToPdfGenerator(
    IExecuteApplicationProcess executeApplicationProcess,
    ILockerService lockerService,
    ILogger<ChromeHtmlToPdfGenerator> logger,
    IAntiXssService antiXssService) : IHtmlToPdfGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    /// <summary>
    ///     High level method that converts HTML to PDF.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePdfFromHtmlAsync(HtmlToPdfGeneratorOptions options,
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

        options.OutputFilePath.AddMetadataToPdfFile(options.DocumentMetadata);

        return log;
    }

    private static string CreateArguments(HtmlToPdfGeneratorOptions options)
    {
        string[] parameters =
        [
            ..ChromeGeneralParameters.GeneralParameters, "--no-pdf-header-footer",
            "--generate-pdf-document-outline", "--export-tagged-pdf",
            string.Create(CultureInfo.InvariantCulture,
                $"--virtual-time-budget={options.VirtualTimeBudgetTotalMilliseconds}")
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
            $" --print-to-pdf=\"{options.OutputFilePath}\" \"{options.SourceHtmlFileOrUri}\" ");

        return arguments.ToString();
    }
}
