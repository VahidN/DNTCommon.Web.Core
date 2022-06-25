using System;
using System.Net;
using System.Net.Sockets;

namespace DNTCommon.Web.Core;

/// <summary>
/// Network Extensions
/// </summary>
public static class NetworkExtensions
{
    /// <summary>
    /// Determines whether ex is a SocketException or WebException
    /// </summary>
    public static bool IsNetworkError(this Exception ex)
    {
        if (ex == null)
        {
            return false;
        }

        return ex is SocketException ||
               ex is WebException ||
               ex.InnerException?.IsNetworkError() == true;
    }
}