namespace DNTCommon.Web.Core;

/// <summary>
///     Represents the ServerInfo
/// </summary>
public class WebServerInfo
{
    /// <summary>
    ///     Provides info about the web server's timezone
    /// </summary>
    public WebServerTimeZone TimeZone { set; get; } = default!;

    /// <summary>
    ///     Provides info about the web server's application
    /// </summary>
    public ApplicationProcess Process { set; get; } = default!;

    /// <summary>
    ///     Provides info about the web server's hardware
    /// </summary>
    public WebServerHardware Hardware { set; get; } = default!;

    /// <summary>
    ///     Provides info about the web server's OS
    /// </summary>
    public WebServerOS OS { set; get; } = default!;

    /// <summary>
    ///     Provides info about the web server's runtime
    /// </summary>
    public WebServerRuntime Runtime { set; get; } = default!;

    /// <summary>
    ///     Gets the system's drive info
    /// </summary>
    public PCDriveInfo DriveInfo { set; get; } = default!;
}