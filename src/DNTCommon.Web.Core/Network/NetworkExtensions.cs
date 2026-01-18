using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        => ex is not null &&
           (ex is SocketException || ex is WebException || ex.InnerException?.IsNetworkError() == true);

    /// <summary>
    ///     Determines if there is at least one network interface capable of reaching the interwebs
    /// </summary>
    /// <returns>True if there is a viable connection</returns>
    public static bool IsNetworkAvailable()
        => NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.GetAllNetworkInterfaces()
            .Any(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up &&
                                     networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                                     networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                     networkInterface.GetIPv4Statistics() is { BytesReceived: > 0, BytesSent: > 0 });

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
            return !hostNameOrAddressToPing.IsEmpty() && IsNetworkAvailable() &&
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

    /// <summary>
    ///     Asynchronously retrieves the expiration date of the SSL/TLS certificate for a given hostname and port.
    /// </summary>
    /// <param name="hostname">The host to connect to.</param>
    /// <param name="port">The port to connect to. The default is 443.</param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The certificate's expiration date in UTC, or null if it cannot be retrieved.</returns>
    public static async Task<DateTime?> GetCertificateExpiryAsync(this string hostname,
        int port = 443,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(hostname, port, cancellationToken);

            // IMPORTANT: We are deliberately ignoring all certificate validation errors.
            // This is acceptable ONLY because our goal is to inspect the certificate,
            // not to establish a secure communication channel for sensitive data.
            // For any other purpose, this would be a major security vulnerability.
            var sslOptions = new SslClientAuthenticationOptions
            {
                TargetHost = hostname,
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            };

            await using var networkStream = client.GetStream();
            await using var sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false);
            await sslStream.AuthenticateAsClientAsync(sslOptions, cancellationToken);
            using var cert = sslStream.RemoteCertificate as X509Certificate2;

            return cert?.NotAfter.ToUniversalTime();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "Failed to GetCertificateExpiryAsync(`{Path}`).", hostname);

            return null;
        }
    }

    /// <summary>
    ///     Gets the port number of the LocalEndPoint(127.0.0.1:0).
    /// </summary>
    /// <returns></returns>
    public static int? FindAvailableNetworkPort()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(IPEndPoint.Parse(s: "127.0.0.1:0"));
        var socketLocalEndPoint = socket.LocalEndPoint;

        return socketLocalEndPoint is IPEndPoint endPoint ? endPoint.Port : null;
    }
}
