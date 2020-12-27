namespace DNTCommon.Web.Core
{
    /// <summary>
    /// SafeFile Download Service
    /// </summary>
    public interface IFileNameSanitizerService
    {
        /// <summary>
        /// Determines whether the requested file is safe to download.
        /// </summary>
        SafeFile IsSafeToDownload(string folderPath, string requestedFileName);
    }
}