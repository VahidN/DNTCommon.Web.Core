namespace DNTCommon.Web.Core;

/// <summary>
///     PCDrive Info Provider
/// </summary>
public static class PCDriveInfoProvider
{
    /// <summary>
    ///     Provides access to information on a drive.
    /// </summary>
    /// <returns></returns>
    public static PCDriveInfo GetDriveInfo()
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
    public static bool IsThereEnoughFreeSpaceOnAppDrive(this long requiredAvailableFreeSpaceInBytes)
        => GetDriveInfo().AvailableFreeSpaceToCurrentUserInBytes >= requiredAvailableFreeSpaceInBytes;
}
