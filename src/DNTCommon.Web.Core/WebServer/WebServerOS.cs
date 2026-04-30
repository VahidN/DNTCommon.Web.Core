namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's OS
/// </summary>
public class WebServerOS
{
    /// <summary>
    ///     Gets the platform architecture on which the current app is running.
    /// </summary>
    public string Architecture { set; get; } = null!;

    /// <summary>
    ///     Gets a string that describes the operating system on which the app is running.
    /// </summary>
    public string Description { set; get; } = null!;

    /// <summary>
    ///     Retrieves all environment variable names and their values from the current process.
    /// </summary>
    public IList<(string Key, string Value)> EnvironmentVariables { set; get; } = null!;

    /// <summary>
    ///     Gets the NetBIOS name of this local computer.
    /// </summary>
    public string ComputerName { set; get; } = null!;

    /// <summary>
    ///     The server's time
    /// </summary>
    public DateTime ServerTime { set; get; }

    /// <summary>
    ///     Gets the username of the person who is associated with the current thread.
    /// </summary>
    public string UserName { set; get; } = null!;

    /// <summary>
    ///     A string that contains the DNS host name of the local computer.
    /// </summary>
    public string HostName { set; get; } = null!;

    /// <summary>
    ///     Returns the Internet Protocol (IP) addresses for the specified host.
    /// </summary>
    public string HostAddresses { set; get; } = null!;

    /// <summary>
    ///     The server's uptime
    /// </summary>
    public TimeSpan UpTime { set; get; } = TimeSpan.Zero;

    /// <summary>
    ///     Sets or gets the number of active connections.
    ///     You can monitor port usage in .NET applications by using it.
    /// </summary>
    public int ActiveTcpConnectionsCount { set; get; }
}
