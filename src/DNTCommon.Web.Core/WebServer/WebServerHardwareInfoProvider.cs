namespace DNTCommon.Web.Core;

/// <summary>
///     WebServer Hardware Info Provider
/// </summary>
public static class WebServerHardwareInfoProvider
{
    /// <summary>
    ///     Provides info about the web server's hardware
    /// </summary>
    /// <returns></returns>
    public static async Task<WebServerHardware> GetWebServerHardwareAsync(CancellationToken ct = default)
    {
        var totalAvailableMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        return new WebServerHardware
        {
            ProcessorCount = Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture),
            MemoryUsage = Environment.WorkingSet.ToFormattedFileSize(),
            MemoryUsageInBytes = Environment.WorkingSet,
            TotalPhysicalMemory = totalAvailableMemoryBytes.ToFormattedFileSize(),
            TotalPhysicalMemoryInBytes = totalAvailableMemoryBytes,
            CpuUsage = await GetCpuUsageTotalAsync(Process.GetCurrentProcess(), ct)
        };
    }

    /// <summary>
    ///     Gets the total processor time for this process.
    /// </summary>
    /// <param name="process"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static async Task<double> GetCpuUsageTotalAsync(Process process, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(process);

        var startTime = DateTime.UtcNow;
        var startCpuUsage = process.TotalProcessorTime;

        await Task.Delay(millisecondsDelay: 500, ct);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = process.TotalProcessorTime;
        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        return cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100.0;
    }
}
