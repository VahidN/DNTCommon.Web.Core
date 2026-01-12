namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's runtime
/// </summary>
public class WebServerRuntime
{
    /// <summary>
    ///     Gets the name of the .NET installation on which an app is running.
    /// </summary>
    public string FrameworkDescription { set; get; } = null!;

    /// <summary>
    ///     Gets the platform for which the runtime was built (or on which an app is running).
    /// </summary>
    public string RuntimeIdentifier { set; get; } = null!;

    /// <summary>
    ///     Gets version information.
    /// </summary>
    public string? InformationalVersion { set; get; }

    /// <summary>
    ///     Returns output of `dotnet sdk check`
    /// </summary>
    public string SdkCheckInfo { set; get; } = null!;

    /// <summary>
    ///     Returns output of `dotnet --info`
    /// </summary>
    public string DotNetInfo { set; get; } = null!;
}
