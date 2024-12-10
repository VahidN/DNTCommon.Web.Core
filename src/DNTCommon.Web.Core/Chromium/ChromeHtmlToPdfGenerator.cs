using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PDF.
/// </summary>
public class ChromeHtmlToPdfGenerator(IExecuteApplicationProcess executeApplicationProcess) : IHtmlToPdfGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    /// <summary>
    ///     High level method that converts HTML to PDF.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePdfFromHtmlAsync(HtmlToPdfGeneratorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string[] parameters = [..ChromeGeneralParameters.GeneralParameters, "--no-pdf-header-footer"];

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

        var appPath = GetChromePath(options);

        if (string.IsNullOrWhiteSpace(appPath))
        {
            throw new InvalidOperationException(ErrorMessage);
        }

        var log = await executeApplicationProcess.ExecuteProcessAsync(new ApplicationStartInfo
        {
            ProcessName = "chrome",
            Arguments = arguments.ToString(),
            AppPath = appPath,
            WaitForExit = options.WaitForExit,
            KillProcessOnStart = true
        });

        options.OutputPdfFile.AddMetadataToPdfFile(options.DocumentMetadata);

        return log;
    }

    private static string? GetChromePath(HtmlToPdfGeneratorOptions options)
        => !string.IsNullOrWhiteSpace(options.ChromeExecutablePath)
            ? options.ChromeExecutablePath
            : ChromeFinder.Find();
}