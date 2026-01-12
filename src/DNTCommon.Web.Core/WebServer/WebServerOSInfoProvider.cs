using System.Collections;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     WebServer OS Info Provider
/// </summary>
public static class WebServerOSInfoProvider
{
    /// <summary>
    ///     Provides info about the web server's OS
    /// </summary>
    /// <returns></returns>
    public static async Task<WebServerOS> GetWebServerOSAsync(CancellationToken cancellationToken = default)
    {
        var hostName = Dns.GetHostName();
        var addresses = await GetIPsAsync(hostName, cancellationToken);

        return new WebServerOS
        {
            Architecture = RuntimeInformation.OSArchitecture.ToString(),
            Description = RuntimeInformation.OSDescription,
            EnvironmentVariables = GetEnvironmentVariables(),
            ComputerName = Environment.MachineName,
            ServerTime = DateTime.UtcNow,
            UserName = Environment.UserName,
            HostName = hostName,
            HostAddresses = string.Join(separator: ", ", addresses),
            UpTime = TimeSpan.FromMilliseconds(Environment.TickCount64),
            ActiveTcpConnectionsCount = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Length
        };
    }

    /// <summary>
    ///     Retrieves all environment variable names and their values from the current process.
    /// </summary>
    /// <returns></returns>
    public static List<(string Key, string Value)> GetEnvironmentVariables()
        =>
        [
            .. from DictionaryEntry entry in Environment.GetEnvironmentVariables()
            let key = entry.Key.ToInvariantString()
            let value = entry.Value.ToInvariantString()
            where !string.IsNullOrWhiteSpace(key)
            select (key, value ?? "")
        ];

    /// <summary>
    ///     Returns the Internet Protocol (IP) addresses for the specified host as an asynchronous operation.
    /// </summary>
    /// <param name="hostName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string[]> GetIPsAsync(string hostName, CancellationToken cancellationToken)
        =>
        [
            .. (await Dns.GetHostAddressesAsync(hostName, cancellationToken))
            .Where(o => o.AddressFamily == AddressFamily.InterNetwork)
            .Select(o => o.ToString())
        ];
}
