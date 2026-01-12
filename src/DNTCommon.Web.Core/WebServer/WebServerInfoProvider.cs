using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     WebServer Info Provider
/// </summary>
public static class WebServerInfoProvider
{
    /// <summary>
    ///     Gets the addresses used by the server.
    /// </summary>
    public static ICollection<string> GetKestrelListeningAddresses(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        ICollection<string>? addresses;

        try
        {
            var server = webApplication.Services.GetRequiredService<IServer>();
            addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        }
        catch
        {
            addresses = null;
        }

        if (addresses is null || addresses.Count == 0)
        {
            addresses = ["http://localhost:5000"];
        }

        return addresses;
    }

    /// <summary>
    ///     Returns the current server's basic hardware and software info
    /// </summary>
    /// <returns></returns>
    public static async Task<WebServerInfo> GetServerInfoAsync(CancellationToken cancellationToken = default)
        => new()
        {
            Process = ApplicationProcessInfoProvider.GetApplicationProcess(),
            Runtime = DotNetInfoProvider.GetWebServerRuntime(),
            OS = await WebServerOSInfoProvider.GetWebServerOSAsync(cancellationToken),
            DriveInfo = PCDriveInfoProvider.GetDriveInfo(),
            TimeZone = WebServerTimeZoneInfoProvider.GetTimezoneDetails(),
            Hardware = await WebServerHardwareInfoProvider.GetWebServerHardwareAsync(cancellationToken),
            LinuxInfo = LinuxInfoProvider.GetLinuxWebServerInfo()
        };
}
