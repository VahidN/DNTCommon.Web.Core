using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public class ChromeHtmlToPngGenerator(
    IExecuteApplicationProcess executeApplicationProcess,
    ILockerService lockerService) : IHtmlToPngGenerator
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

        if (options is { ResizeImageOptions: not null })
        {
            var newData = options.OutputPngFile.ResizeImage(options.ResizeImageOptions);

            if (newData is not null)
            {
                await File.WriteAllBytesAsync(options.OutputPngFile, newData);
            }
        }

        return log;
    }

    private static string? GetChromePath(HtmlToPngGeneratorOptions options)
        => !string.IsNullOrWhiteSpace(options.ChromeExecutablePath)
            ? options.ChromeExecutablePath
            : ChromeFinder.Find();
}