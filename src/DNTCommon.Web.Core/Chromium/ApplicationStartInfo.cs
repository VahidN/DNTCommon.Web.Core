namespace DNTCommon.Web.Core;

/// <summary>
///     Represents the data with which to start the process
/// </summary>
public class ApplicationStartInfo
{
    /// <summary>
    ///     The friendly name of the process.
    /// </summary>
    public string? ProcessName { set; get; }

    /// <summary>
    ///     Gets or sets the set of command-line arguments to use when starting the application.
    /// </summary>
    public string? Arguments { set; get; }

    /// <summary>
    ///     Gets or sets the application or document to start.
    /// </summary>
    public string? AppPath { set; get; }

    /// <summary>
    ///     The time span to wait before killing this app.
    ///     Its default value is 10 seconds.
    /// </summary>
    public TimeSpan WaitForExit { set; get; } = TimeSpan.FromSeconds(value: 10);

    /// <summary>
    ///     Should we kill all related processes on start
    /// </summary>
    public bool KillProcessOnStart { set; get; }
}