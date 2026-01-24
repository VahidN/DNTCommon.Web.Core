namespace DNTCommon.Web.Core;

/// <summary>
///     Provides access to the current Linux server info
/// </summary>
public class LinuxWebServerInfo
{
    /// <summary>
    ///     Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public IList<string> EnvironmentVariables { set; get; } = [];

    /// <summary>
    ///     Provides output of `apt search dotnet-sdk*`
    /// </summary>
    public IList<Version> AvailableSdkVersions { set; get; } = [];
}
