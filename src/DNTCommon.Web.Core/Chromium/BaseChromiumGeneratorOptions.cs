namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlToPdfGenerator Options
/// </summary>
public class BaseChromiumGeneratorOptions
{
    /// <summary>
    ///     Source Uri
    /// </summary>
    public string? SourceHtmlFileOrUri { set; get; }

    /// <summary>
    ///     Output path
    /// </summary>
    public string? OutputFilePath { set; get; }

    /// <summary>
    ///     If it's not specified, ChromeFinder.Find with try to find it!
    /// </summary>
    public string? ChromeExecutablePath { get; set; } = "";

    /// <summary>
    ///     Wait for exit.
    /// </summary>
    public TimeSpan? WaitForExit { set; get; }

    /// <summary>
    ///     Sets --virtual-time-budget of chrome. Its default value is 30 minutes.
    /// </summary>
    public TimeSpan VirtualTimeBudget { set; get; } = TimeSpan.FromMinutes(value: 30);

    /// <summary>
    ///     Should we kill all related processes on start?
    /// </summary>
    public bool KillProcessOnStart { set; get; }

    /// <summary>
    ///     Tries to find Chrome
    /// </summary>
    public string? ChromePath
        => !string.IsNullOrWhiteSpace(ChromeExecutablePath) ? ChromeExecutablePath : ChromeFinder.Find();

    /// <summary>
    ///     returns --virtual-time-budget of chrome.
    /// </summary>
    public int VirtualTimeBudgetTotalMilliseconds => VirtualTimeBudget.TotalMilliseconds < int.MaxValue
        ? (int)VirtualTimeBudget.TotalMilliseconds
        : int.MaxValue - 1;
}