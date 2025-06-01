using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Disallows uploading dangerous files such as .aspx, web.config and .asp files.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class AllowUploadSafeFilesAttribute : UploadFileValidationBaseAttribute
{
    private readonly HashSet<string> _extensionsToFilter = new(StringComparer.OrdinalIgnoreCase)
    {
        ".aspx",
        ".asax",
        ".asp",
        ".ashx",
        ".asmx",
        ".axd",
        ".master",
        ".svc",
        ".php",
        ".php3",
        ".php4",
        ".ph3",
        ".ph4",
        ".php4",
        ".ph5",
        ".sphp",
        ".cfm",
        ".ps",
        ".stm",
        ".htaccess",
        ".htpasswd",
        ".php5",
        ".phtml",
        ".cgi",
        ".pl",
        ".plx",
        ".py",
        ".rb",
        ".sh",
        ".jsp",
        ".cshtml",
        ".vbhtml",
        ".swf",
        ".xap",
        ".asptxt",
        ".xamlx"
    };

    private readonly HashSet<string> _namesToFilter = new(StringComparer.OrdinalIgnoreCase)
    {
        "web.config",
        "htaccess",
        "htpasswd",
        "web~1.con",
        "desktop.ini"
    };

    /// <summary>
    ///     Disallows uploading dangerous files such as .aspx, web.config and .asp files.
    /// </summary>
    /// <param name="allowUploadEmptyFiles">Determines whether empty files can be uploaded</param>
    /// <param name="extensionsToFilter">Disallowed file extensions such as .asp</param>
    /// <param name="namesToFilter">Disallowed names such as web.config</param>
    /// <param name="maxFileSizeInBytes">Max allowed file size. It will be ignored if it's null.</param>
    /// <param name="minFileSizeInBytes">Min allowed file size. It will be ignored if it's null.</param>
    public AllowUploadSafeFilesAttribute(bool allowUploadEmptyFiles = false,
        string[]? extensionsToFilter = null,
        string[]? namesToFilter = null,
        long? maxFileSizeInBytes = null,
        long? minFileSizeInBytes = null)
    {
        AllowUploadEmptyFiles = allowUploadEmptyFiles;
        MaxFileSizeInBytes = maxFileSizeInBytes;
        MinFileSizeInBytes = minFileSizeInBytes;

        if (extensionsToFilter is not null)
        {
            foreach (var item in extensionsToFilter)
            {
                _extensionsToFilter.Add(item);
            }
        }

        if (namesToFilter is not null)
        {
            foreach (var item in namesToFilter)
            {
                _namesToFilter.Add(item);
            }
        }

        ExtensionsToFilter = extensionsToFilter;
        NamesToFilter = namesToFilter;
    }

    /// <summary>
    ///     Disallowed file extensions such as .asp
    /// </summary>
    /// <value></value>
    public string[]? ExtensionsToFilter { get; }

    /// <summary>
    ///     Disallowed names such as web.config
    /// </summary>
    /// <value></value>
    public string[]? NamesToFilter { get; }

    /// <summary>
    ///     A custom error message for FileName
    /// </summary>
    public string? FileNameIsEmptyErrorMessage { set; get; }

    /// <summary>
    ///     A custom error message for FileExtensions
    /// </summary>
    public string? FileExtensionsErrorMessage { set; get; }

    /// <summary>
    ///     Validates the input file
    /// </summary>
    public override (bool Success, string? ErrorMessage) IsValidFile(IFormFile? file)
    {
        if (file is null)
        {
            return (!IsRequired, IsRequiredErrorMessage ?? ErrorMessage);
        }

        if (file.Length == 0)
        {
            return (AllowUploadEmptyFiles, AllowUploadEmptyFilesErrorMessage ?? ErrorMessage);
        }

        if (MaxFileSizeInBytes.HasValue && file.Length > MaxFileSizeInBytes.Value)
        {
            return (false, MaxFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        if (MinFileSizeInBytes.HasValue && file.Length < MinFileSizeInBytes.Value)
        {
            return (false, MinFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        var fileName = file.FileName;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return (false, FileNameIsEmptyErrorMessage ?? ErrorMessage);
        }

        fileName = fileName.ToLowerInvariant();
        var name = Path.GetFileName(fileName);
        var ext = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, FileNameIsEmptyErrorMessage ?? ErrorMessage);
        }

        //for "file.asp;.jpg" files --> run as an ASP file
        return (
            !_extensionsToFilter.Contains(ext) && !_namesToFilter.Contains(name) && !_namesToFilter.Contains(ext) &&
            _extensionsToFilter.All(item => !name.Contains(item, StringComparison.OrdinalIgnoreCase)),
            FileExtensionsErrorMessage ?? ErrorMessage);
    }
}
