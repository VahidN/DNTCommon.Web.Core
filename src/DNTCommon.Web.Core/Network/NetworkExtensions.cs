using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Network Extensions
/// </summary>
public static class NetworkExtensions
{
    /// <summary>
    ///     Determines whether ex is a SocketException or WebException
    /// </summary>
    public static bool IsNetworkError(this Exception? ex)
        => ex != null && (ex is SocketException || ex is WebException || ex.InnerException?.IsNetworkError() == true);

    /// <summary>
    ///     Is there any internet connection?
    /// </summary>
    /// <returns></returns>
    public static bool IsConnectedToInternet(TimeSpan timeout,
        string? hostNameOrAddressToPing = "www.google.com",
        ILogger? logger = null)
    {
        try
        {
            return !hostNameOrAddressToPing.IsEmpty() && NetworkInterface.GetIsNetworkAvailable() &&
                   hostNameOrAddressToPing.PingHost(timeout).Status == IPStatus.Success;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "Ping Error!");

            return false;
        }
    }

    /// <summary>
    ///     Attempts to send an Internet Control Message Protocol (ICMP) echo message
    /// </summary>
    /// <returns></returns>
    public static PingReply PingHost(this string hostNameOrAddress, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddress);

        using var ping = new Ping();

        return ping.Send(hostNameOrAddress.GetHostIPV4(), (int)timeout.TotalMilliseconds);
    }

    /// <summary>
    ///     Gets the current system's IPV4
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIPV4() => Dns.GetHostName().GetHostIPV4();

    /// <summary>
    ///     Gets the given host's IPV4
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetHostIPV4(this string host)
    {
        ArgumentNullException.ThrowIfNull(host);

        return Dns.GetHostEntry(host).AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);
    }
}