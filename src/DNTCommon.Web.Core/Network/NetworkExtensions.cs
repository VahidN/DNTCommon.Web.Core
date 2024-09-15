using System.Net.NetworkInformation;
using System.Net.Sockets;

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
    {
        if (ex == null)
        {
            return false;
        }

        return ex is SocketException || ex is WebException || ex.InnerException?.IsNetworkError() == true;
    }

    /// <summary>
    ///     Is there any internet connection?
    /// </summary>
    /// <returns></returns>
    public static bool IsConnectedToInternet()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            return false;
        }

        try
        {
            using var ping = new Ping();

            return ping.Send(hostNameOrAddress: "www.google.com", timeout: 2000).Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
}