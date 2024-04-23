using System.Collections;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     WebServer Info Provider
/// </summary>
public static class WebServerInfoProvider
{
    /// <summary>
    ///     Returns the current server's basic hardware and software info
    /// </summary>
    /// <returns></returns>
    public static async Task<WebServerInfo> GetServerInfoAsync()
    {
        var process = Process.GetCurrentProcess();

        var version = (AssemblyInformationalVersionAttribute?)Assembly.GetExecutingAssembly()
            .GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute));

        var hostName = Dns.GetHostName();

        var addresses = (await Dns.GetHostAddressesAsync(hostName))
            .Where(o => o.AddressFamily == AddressFamily.InterNetwork)
            .Select(o => o.ToString())
            .ToArray();

        return new WebServerInfo
        {
            Process = new ApplicationProcess
            {
                ProcessArguments = string.Join(' ', Environment.GetCommandLineArgs()),
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
                HostAddresses = string.Join(", ", addresses)
            },
            DriveInfo = GetDriveInfo(),
            TimeZone = GetTimezoneDetails(),
            Hardware = new WebServerHardware
            {
                ProcessorCount = Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture),
                MemoryUsage = Environment.WorkingSet.ToFormattedFileSize(),
                TotalPhysicalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes.ToFormattedFileSize(),
                CpuUsage = await GetCpuUsageTotalAsync(process)
            }
        };
    }

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

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = process.TotalProcessorTime;
        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        return cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100.0;
    }

    private static List<(string Key, string Value)> GetEnvironmentVariables()
        => (from DictionaryEntry entry in Environment.GetEnvironmentVariables()
            let key = entry.Key.ToString()
            let value = entry.Value?.ToString()
            where !string.IsNullOrWhiteSpace(key)
            select (key, value ?? "")).ToList();

    private static PCDriveInfo GetDriveInfo()
    {
        var currentDrive = Array.Find(DriveInfo.GetDrives(),
            driveInfo => string.Equals(driveInfo.RootDirectory.FullName,
                Directory.GetDirectoryRoot(Path.GetPathRoot(Environment.ProcessPath!)!),
                StringComparison.OrdinalIgnoreCase))!;

        return new PCDriveInfo
        {
            Drive = currentDrive.Name,
            VolumeLabel = currentDrive.VolumeLabel,
            FileSystem = currentDrive.DriveFormat,
            AvailableSpaceToCurrentUser = currentDrive.AvailableFreeSpace.ToFormattedFileSize(),
            TotalAvailableSpace = currentDrive.TotalFreeSpace.ToFormattedFileSize(),
            TotalSizeOfDive = currentDrive.TotalSize.ToFormattedFileSize()
        };
    }
}