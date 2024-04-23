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
    ///     Gets the amount of physical memory mapped to the process context.
    /// </summary>
    public string MemoryUsage { set; get; } = default!;

    /// <summary>
    ///     Gets the total available memory for the garbage collector to use when the last garbage collection occurred.
    /// </summary>
    public string TotalPhysicalMemory { set; get; } = default!;

    /// <summary>
    ///     Get the Cpu Usage
    /// </summary>
    public double CpuUsage { set; get; }
}