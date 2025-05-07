using System.Collections;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
    public static async Task<WebServerInfo> GetServerInfoAsync()
    {
        var process = Process.GetCurrentProcess();
        var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var hostName = Dns.GetHostName();
        var addresses = await GetIPsAsync(hostName);
        var totalAvailableMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return new WebServerInfo
        {
            Process = new ApplicationProcess
            {
                ProcessArguments = string.Join(separator: ' ', Environment.GetCommandLineArgs()),
                ProcessPath = Environment.ProcessPath ?? "",
                ProcessName = process.MainModule?.ModuleName ?? process.ProcessName,
                ProcessId = process.Id.ToString(CultureInfo.InvariantCulture),
                ProcessStartTime = process.StartTime,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString()
            },
            Runtime = new WebServerRuntime
            {
                InformationalVersion = version?.InformationalVersion,
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier
            },
            OS = new WebServerOS
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
            },
            DriveInfo = GetDriveInfo(),
            TimeZone = GetTimezoneDetails(),
            Hardware = new WebServerHardware
            {
                ProcessorCount = Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture),
                MemoryUsage = Environment.WorkingSet.ToFormattedFileSize(),
                MemoryUsageInBytes = Environment.WorkingSet,
                TotalPhysicalMemory = totalAvailableMemoryBytes.ToFormattedFileSize(),
                TotalPhysicalMemoryInBytes = totalAvailableMemoryBytes,
                CpuUsage = await GetCpuUsageTotalAsync(process)
            }
        };
    }

    private static async Task<string[]> GetIPsAsync(string hostName)
        =>
        [
            .. (await Dns.GetHostAddressesAsync(hostName)).Where(o => o.AddressFamily == AddressFamily.InterNetwork)
            .Select(o => o.ToString())
        ];

    private static WebServerTimeZone GetTimezoneDetails()
    {
        var timeZoneInfo = TimeZoneInfo.Local;

        return new WebServerTimeZone
        {
            DisplayName = timeZoneInfo.DisplayName,
            BaseUtcOffset = timeZoneInfo.BaseUtcOffset,
            Language = CultureInfo.CurrentUICulture.Name,
            IsDaylightSavingTime = timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow)
        };
    }

    private static async Task<double> GetCpuUsageTotalAsync(Process process)
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = process.TotalProcessorTime;

        await Task.Delay(millisecondsDelay: 500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = process.TotalProcessorTime;
        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        return cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100.0;
    }

    private static List<(string Key, string Value)> GetEnvironmentVariables()
        =>
        [
            .. from DictionaryEntry entry in Environment.GetEnvironmentVariables()
            let key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture)
            let value = Convert.ToString(entry.Value, CultureInfo.InvariantCulture)
            where !string.IsNullOrWhiteSpace(key)
            select (key, value ?? "")
        ];

    private static PCDriveInfo GetDriveInfo()
    {
        var currentDrive = Array.Find(DriveInfo.GetDrives(),
            driveInfo => string.Equals(driveInfo.RootDirectory.FullName,
                Directory.GetDirectoryRoot(Path.GetPathRoot(Environment.ProcessPath)!),
                StringComparison.OrdinalIgnoreCase))!;

        return new PCDriveInfo
        {
            Drive = currentDrive.Name,
            VolumeLabel = currentDrive.VolumeLabel,
            FileSystem = currentDrive.DriveFormat,
            AvailableSpaceToCurrentUser = currentDrive.AvailableFreeSpace.ToFormattedFileSize(),
            AvailableFreeSpaceToCurrentUserInBytes = currentDrive.AvailableFreeSpace,
            TotalAvailableSpace = currentDrive.TotalFreeSpace.ToFormattedFileSize(),
            TotalAvailableSpaceInBytes = currentDrive.TotalFreeSpace,
            TotalSizeOfDive = currentDrive.TotalSize.ToFormattedFileSize(),
            TotalSizeOfDiveInBytes = currentDrive.TotalSize
        };
    }

    /// <summary>
    ///     Do we have enough free-space available on the application's drive to work with it?
    /// </summary>
    /// <param name="requiredAvailableFreeSpaceInBytes">How much free-space should be left to raise an alarm?</param>
    public static bool IsThereEnoughFreeSpaceOnAppDrive(long requiredAvailableFreeSpaceInBytes)
        => GetDriveInfo().AvailableFreeSpaceToCurrentUserInBytes >= requiredAvailableFreeSpaceInBytes;
}
