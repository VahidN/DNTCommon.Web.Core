namespace DNTCommon.Web.Core;

/// <summary>
/// Safe File
/// </summary>
public class SafeFile
{
    /// <summary>
    /// Determines whether the requested file is safe to download.
    /// </summary>
    public bool IsSafeToDownload { get; set; }

    /// <summary>
    /// Cleaned requested file's name.
    /// </summary>
    public string SafeFileName { get; set; } = string.Empty;

    /// <summary>
    /// Cleaned requested file's path.
    /// </summary>
    public string SafeFilePath { get; set; } = string.Empty;
}