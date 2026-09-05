using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     IP Address Validation
/// </summary>
public static class LocalIp
{
    /// <summary>
    ///     ::1 or null-IP v6
    /// </summary>
    public const string NullIPv6 = "::1";

    /// <summary>
    ///     Determines whether the given string is a valid IP version 4 or 6 address.
    /// </summary>
    public static bool IsValidIp(this string? ip)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            return false;
        }

        return ipAddress.AddressFamily switch
        {
            //Address for IP version 4 or 6
            AddressFamily.InterNetwork or AddressFamily.InterNetworkV6 => true,
            _ => false
        };
    }

    /// <summary>
    ///     Determines whether the given string is a valid IP version 4 or 6 address.
    /// </summary>
    public static bool IsValidIp(this IPAddress? ipAddress)
    {
        if (ipAddress is null)
        {
            return false;
        }

        return ipAddress.AddressFamily switch
        {
            //Address for IP version 4 or 6
            AddressFamily.InterNetwork or AddressFamily.InterNetworkV6 => true,
            _ => false
        };
    }

    /// <summary>
    ///     Determines whether the given string is a valid IP version 4 address.
    /// </summary>
    public static bool IsValidIpV4(this string? ip)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            return false;
        }

        return ipAddress.AddressFamily switch
        {
            AddressFamily.InterNetwork => true,
            _ => false
        };
    }

    /// <summary>
    ///     Determines whether the given string is a valid IP version 6 address.
    /// </summary>
    public static bool IsValidIpV6(this string? ip)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            return false;
        }

        return ipAddress.AddressFamily switch
        {
            AddressFamily.InterNetworkV6 => true,
            _ => false
        };
    }

    /// <summary>
    ///     Determines whether the specified `ip` address is a private IP address.
    /// </summary>
    public static bool IsLocalIp(this string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            return false;
        }

        return ip.IsInSubnet(cidr: "127.0.0.1/8") || ip.IsInSubnet(cidr: "10.0.0.0/8") ||
               ip.IsInSubnet(cidr: "172.16.0.0/12") || ip.IsInSubnet(cidr: "192.168.0.0/16") ||
               ip.IsInSubnet(cidr: "169.254.0.0/16 ") || string.Equals(ip, NullIPv6, StringComparison.Ordinal);
    }

    /// <summary>
    ///     IP Address Validation Using CIDR
    ///     http://social.msdn.microsoft.com/forums/en-US/netfxnetcom/thread/29313991-8b16-4c53-8b5d-d625c3a861e1/
    /// </summary>
    public static bool IsInSubnet(this string ipAddress, string cidr)
    {
        if (string.IsNullOrWhiteSpace(cidr))
        {
            throw new ArgumentNullException(nameof(cidr));
        }

        var parts = cidr.Split(separator: '/');
        var baseAddress = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), startIndex: 0);
        var address = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), startIndex: 0);
        var mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1], CultureInfo.InvariantCulture)));

        return (baseAddress & mask) == (address & mask);
    }

    /// <summary>
    ///     Identifies local requests
    /// </summary>
    public static bool IsLocal(this ConnectionInfo? conn)
    {
        if (conn?.RemoteIpAddress?.IsSet() == false)
        {
            return true;
        }

        if (conn?.RemoteIpAddress is not null && conn.LocalIpAddress?.IsSet() == true)
        {
            return conn.RemoteIpAddress.Equals(conn.LocalIpAddress);
        }

        return conn?.RemoteIpAddress?.IsLoopback() ?? false;
    }

    /// <summary>
    ///     Identifies local requests
    /// </summary>
    public static bool IsLocal(this HttpContext? ctx) => ctx?.Connection.IsLocal() == true;

    /// <summary>
    ///     Identifies local requests
    /// </summary>
    public static bool IsLocal(this HttpRequest? req) => req?.HttpContext.IsLocal() == true;

    /// <summary>
    ///     address is not nullIPv6
    /// </summary>
    public static bool IsSet(this IPAddress? address)
        => address is not null && !string.Equals(address.ToString(), NullIPv6, StringComparison.Ordinal);

    /// <summary>
    ///     Indicates whether the specified IP address is the loopback address.
    /// </summary>
    public static bool IsLoopback(this IPAddress? address) => address is not null && IPAddress.IsLoopback(address);

    public static bool IsPublicIp(this IPAddress? address)
    {
        if (address is null)
        {
            return false;
        }
		
        if (IPAddress.IsLoopback(address))
        {
            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4
        {
            var bytes = address.GetAddressBytes();

            // 10.0.0.0/8
            if (bytes[0] == 10)
            {
                return false;
            }

            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            {
                return false;
            }

            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
            {
                return false;
            }

            // 169.254.0.0/16 (link-local)
            if (bytes[0] == 169 && bytes[1] == 254)
            {
                return false;
            }

            return true;
        }
        else // IPv6
        {
            var bytes = address.GetAddressBytes();

            // fc00::/7 Unique local addresses
            if ((bytes[0] & 0xfe) == 0xfc)
            {
                return false;
            }

            // fe80::/10 link-local
            if (bytes[0] == 0xfe && (bytes[1] & 0xc0) == 0x80)
            {
                return false;
            }

            // ::1 loopback handled above
            return true;
        }
    }
}
