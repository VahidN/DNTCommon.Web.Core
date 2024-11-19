using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public class ChromeHtmlToPngGenerator(IExecuteApplicationProcess executeApplicationProcess) : IHtmlToPngGenerator
{
    private const string ErrorMessage = "ChromeFinder was not successful and ChromeExecutablePath is null.";

    private static readonly string[] GeneralParameters =
    [
        "--disable-crash-reporter", "--disable-translate", "--disable-background-networking",
        "--enable-features=NetworkService,NetworkServiceInProcess", "--disable-background-timer-throttling",
        "--disable-backgrounding-occluded-windows", "--disable-breakpad",
        "--disable-client-side-phishing-detection", "--disable-component-extensions-with-background-pages",
        "--disable-default-apps", "--disable-dev-shm-usage", "--disable-component-update", "--disable-extensions",
        "--disable-features=Translate,BackForwardCache,AcceptCHFrame,MediaRouter,OptimizationHints,TranslateUI",
        "--disable-hang-monitor", "--disable-ipc-flooding-protection", "--disable-popup-blocking",
        "--disable-prompt-on-repost", "--disable-renderer-backgrounding", "--disable-sync",
        "--force-color-profile=srgb", "--metrics-recording-only", "--no-first-run", "--enable-automation",
        "--password-store=basic", "--use-mock-keychain", "--enable-blink-features=IdleDetection",
        "--auto-open-devtools-for-tabs", "--headless=old", "--hide-scrollbars", "--mute-audio",
        "--proxy-server='direct://'", "--proxy-bypass-list=*", "--no-sandbox", "--no-zygote",
        "--no-default-browser-check", "--disable-site-isolation-trials", "--no-experiments",
        "--ignore-gpu-blocklist", "--ignore-certificate-errors", "--ignore-certificate-errors-spki-list",
        "--disable-gpu", "--num-raster-threads=2", "--no-service-autorun", "--disable-extensions",
        "--disable-default-apps", "--enable-features=NetworkService", "--disable-setuid-sandbox", "--disable-webgl",
        "--disable-threaded-animation", "--disable-threaded-scrolling", "--disable-in-process-stack-traces",
        "--disable-histogram-customizer", "--disable-gl-extensions", "--disable-composited-antialiasing",
        "--disable-canvas-aa", "--disable-3d-apis", "--disable-accelerated-2d-canvas",
        "--disable-accelerated-jpeg-decoding", "--disable-accelerated-mjpeg-decode",
        "--disable-app-list-dismiss-on-blur", "--disable-accelerated-video-decode"
    ];

    /// <summary>
    ///     High level method that converts HTML to PNG.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string[] parameters = [..GeneralParameters, Invariant($"--window-size=\"{options.Width},{options.Height}\"")];

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
    {
        var chromePath = options.ChromeExecutablePath;

        return !string.IsNullOrWhiteSpace(chromePath) ? chromePath : ChromeFinder.Find();
    }
}