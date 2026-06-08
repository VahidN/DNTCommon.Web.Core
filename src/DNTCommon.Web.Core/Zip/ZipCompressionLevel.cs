namespace DNTCommon.Web.Core;

/// <summary>
///     Defines the compression level options for zip file creation.
/// </summary>
public enum ZipCompressionLevel
{
    /// <summary>
    ///     No compression (0). Fastest but largest file size.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Compression level 1. Minimal compression.
    /// </summary>
    Fastest = 1,

    /// <summary>
    ///     Compression level 2.
    /// </summary>
    VeryFast = 2,

    /// <summary>
    ///     Compression level 3.
    /// </summary>
    Fast = 3,

    /// <summary>
    ///     Compression level 4.
    /// </summary>
    MediumFast = 4,

    /// <summary>
    ///     Compression level 5 (Default). Balanced between speed and compression.
    /// </summary>
    Medium = 5,

    /// <summary>
    ///     Compression level 6.
    /// </summary>
    MediumSlow = 6,

    /// <summary>
    ///     Compression level 7.
    /// </summary>
    Slow = 7,

    /// <summary>
    ///     Compression level 8.
    /// </summary>
    VerySlow = 8,

    /// <summary>
    ///     Compression level 9 (Maximum compression). Slowest but smallest file size.
    /// </summary>
    Maximum = 9
}
