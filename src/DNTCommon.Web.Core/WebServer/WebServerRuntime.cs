namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's runtime
/// </summary>
public class WebServerRuntime
{
    /// <summary>
    ///     Gets the name of the .NET installation on which an app is running.
    /// </summary>
    public string FrameworkDescription { set; get; } = default!;

    /// <summary>
    ///     Gets the platform for which the runtime was built (or on which an app is running).
    /// </summary>
    public string RuntimeIdentifier { set; get; } = default!;

    /// <summary>
    ///     Gets version information.
    /// </summary>
    public string? InformationalVersion { set; get; }
}