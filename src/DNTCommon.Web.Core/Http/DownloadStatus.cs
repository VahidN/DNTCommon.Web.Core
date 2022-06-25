using System;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core;

/// <summary>
/// Download Status
/// </summary>
public class DownloadStatus
{
    /// <summary>
    /// Estimated Remote FileSize
    /// </summary>
    public long RemoteFileSize { get; set; }

    /// <summary>
    /// Current downloaded bytes.
    /// </summary>
    public long BytesTransferred { get; set; }

    /// <summary>
    /// Download's start time.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Received remote file name from the server.
    /// </summary>
    public string RemoteFileName { get; set; } = default!;

    /// <summary>
    /// Current task's status.
    /// </summary>
    public TaskStatus Status { get; set; }

    /// <summary>
    /// Current percent of download operation.
    /// </summary>
    public decimal PercentComplete { get; set; }

    /// <summary>
    /// Download speed.
    /// </summary>
    public double BytesPerSecond { get; set; }

    /// <summary>
    /// Estimated Completion Time
    /// </summary>
    public TimeSpan EstimatedCompletionTime { get; set; }

    /// <summary>
    /// Elapsed Download Time
    /// </summary>
    public TimeSpan ElapsedDownloadTime { get; set; }
}