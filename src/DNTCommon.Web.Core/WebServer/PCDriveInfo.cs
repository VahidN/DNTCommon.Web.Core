namespace DNTCommon.Web.Core;

/// <summary>
///     Provides access to information on a drive.
/// </summary>
public class PCDriveInfo
{
    /// <summary>
    ///     Gets the name of a drive, such as C:
    /// </summary>
    public string Drive { set; get; } = default!;

    /// <summary>
    ///     Gets or sets the volume label of a drive.
    /// </summary>
    public string VolumeLabel { set; get; } = default!;

    /// <summary>
    ///     Gets the name of the file system, such as NTFS or FAT32.
    /// </summary>
    public string FileSystem { set; get; } = default!;

    /// <summary>
    ///     Indicates the formatted amount of available free space on a drive
    /// </summary>
    public string AvailableSpaceToCurrentUser { set; get; } = default!;

    /// <summary>
    ///     Indicates the amount of available free space on a drive, in bytes.
    /// </summary>
    public long AvailableFreeSpaceToCurrentUserInBytes { set; get; }

    /// <summary>
    ///     Gets the formatted total amount of free space available on a drive
    /// </summary>
    public string TotalAvailableSpace { set; get; } = default!;
 /// <summary>
    ///     Gets the total amount of free space available on a drive, in bytes.
    /// </summary>
    public long TotalAvailableSpaceInBytes { set; get; }

    /// <summary>
    ///     Gets the formatted total size of storage space on a drive
    /// </summary>
    public string TotalSizeOfDive { set; get; } = default!;
 /// <summary>
    ///     Gets the total size of storage space on a drive, in bytes.
    /// </summary>
    public long TotalSizeOfDiveInBytes { set; get; }
}
