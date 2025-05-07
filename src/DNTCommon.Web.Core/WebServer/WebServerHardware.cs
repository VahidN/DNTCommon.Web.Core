namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's hardware
/// </summary>
public class WebServerHardware
{
    /// <summary>
    ///     Gets the number of processors available to the current process.
    /// </summary>
    public string ProcessorCount { set; get; } = default!;

    /// <summary>
    ///     Gets the formatted amount of physical memory mapped to the process context.
    /// </summary>
    public string MemoryUsage { set; get; } = default!;
 /// <summary>
    ///     A 64-bit signed integer containing the number of bytes of physical memory mapped to the process context.
    /// </summary>
    public long MemoryUsageInBytes { set; get; }

    /// <summary>
    ///     Gets the formatted total available memory for the garbage collector to use when the last garbage collection
    ///     occurred.
    /// </summary>
    public string TotalPhysicalMemory { set; get; } = default!;
 /// <summary>
    /// </summary>
    public long TotalPhysicalMemoryInBytes { set; get; }

    /// <summary>
    ///     Get the Cpu Usage
    /// </summary>
    public double CpuUsage { set; get; }
}
