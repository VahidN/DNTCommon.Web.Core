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
    ///     Disallowed file extensions such as .asp
    /// </summary>
    /// <value></value>
    public string[]? ExtensionsToFilter { get; set; }

    /// <summary>
    ///     Disallowed names such as web.config
    /// </summary>
    /// <value></value>
    public string[]? NamesToFilter { get; set; }

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
        var (success, errorMessage) = HasValidFileSize(file);

        if (!success)
        {
            return (success, ErrorMessage: errorMessage);
        }

        var fileName = file?.FileName;

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

        if (ExtensionsToFilter is not null)
        {
            foreach (var item in ExtensionsToFilter)
            {
                _extensionsToFilter.Add(item);
            }
        }

        if (NamesToFilter is not null)
        {
            foreach (var item in NamesToFilter)
            {
                _namesToFilter.Add(item);
            }
        }

        //for "file.asp;.jpg" files --> run as an ASP file
        return (
            !_extensionsToFilter.Contains(ext) && !_namesToFilter.Contains(name) && !_namesToFilter.Contains(ext) &&
            _extensionsToFilter.All(item => !name.Contains(item, StringComparison.OrdinalIgnoreCase)),
            FileExtensionsErrorMessage ?? ErrorMessage);
    }
}
