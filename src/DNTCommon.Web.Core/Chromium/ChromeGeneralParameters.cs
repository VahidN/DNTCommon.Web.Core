namespace DNTCommon.Web.Core;

/// <summary>
///     Chrome Headless mode params
///     https://developer.chrome.com/docs/chromium/headless#headless-specific-command-line-flags
/// </summary>
public static class ChromeGeneralParameters
{
    /// <summary>
    ///     Chrome Headless mode params
    ///     https://developer.chrome.com/docs/chromium/headless#headless-specific-command-line-flags
    /// </summary>
    public static readonly string[] GeneralParameters =
    [
        "--disable-search-engine-choice-screen", "--disable-component-extensions-with-background-pages",
        "--allow-pre-commit-input", "--disable-infobars", "--disable-crash-reporter", "--disable-translate",
        "--disable-background-networking", "--enable-features=NetworkService,NetworkServiceInProcess",
        "--disable-background-timer-throttling", "--disable-backgrounding-occluded-windows", "--disable-breakpad",
        "--disable-client-side-phishing-detection", "--disable-component-extensions-with-background-pages",
        "--disable-default-apps", "--disable-dev-shm-usage", "--disable-component-update", "--disable-extensions",
        "--disable-features=Translate,BackForwardCache,AcceptCHFrame,MediaRouter,OptimizationHints,TranslateUI",
        "--disable-hang-monitor", "--disable-ipc-flooding-protection", "--disable-popup-blocking",
        "--disable-prompt-on-repost", "--disable-renderer-backgrounding", "--disable-sync",
        "--force-color-profile=srgb", "--metrics-recording-only", "--no-first-run", "--enable-automation",
        "--password-store=basic", "--use-mock-keychain", "--enable-blink-features=IdleDetection",
        "--auto-open-devtools-for-tabs", "--headless", "--hide-scrollbars", "--mute-audio",
        "--proxy-server='direct://'", "--proxy-bypass-list=*", "--no-sandbox", "--no-zygote",
        "--no-default-browser-check", "--disable-site-isolation-trials", "--no-experiments",
        "--ignore-gpu-blocklist", "--ignore-certificate-errors", "--ignore-certificate-errors-spki-list",
        "--disable-gpu", "--num-raster-threads=2", "--no-service-autorun", "--disable-extensions",
        "--disable-default-apps", "--enable-features=NetworkService", "--disable-setuid-sandbox", "--disable-webgl",
        "--disable-threaded-animation", "--disable-threaded-scrolling", "--disable-in-process-stack-traces",
        "--disable-histogram-customizer", "--disable-gl-extensions", "--disable-composited-antialiasing",
        "--disable-canvas-aa", "--disable-3d-apis", "--disable-accelerated-2d-canvas",
        "--disable-accelerated-jpeg-decoding", "--disable-accelerated-mjpeg-decode",
        "--disable-app-list-dismiss-on-blur", "--disable-accelerated-video-decode",
        "--run-all-compositor-stages-before-draw", "--virtual-time-budget=30000", "--no-crashpad"
    ];
}