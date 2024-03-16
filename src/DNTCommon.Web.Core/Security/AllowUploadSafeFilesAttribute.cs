using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Disallows uploading dangerous files such as .aspx, web.config and .asp files.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class AllowUploadSafeFilesAttribute : ValidationAttribute
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
    /// <param name="extensionsToFilter">Disallowed file extensions such as .asp</param>
    /// <param name="namesToFilter">Disallowed names such as web.config</param>
    public AllowUploadSafeFilesAttribute(string[]? extensionsToFilter = null, string[]? namesToFilter = null)
    {
        if (extensionsToFilter != null)
        {
            foreach (var item in extensionsToFilter)
            {
                _extensionsToFilter.Add(item);
            }
        }

        if (namesToFilter != null)
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
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true; // returning false, makes this field required.
        }

        if (value is IFormFile file)
        {
            return IsValidFile(file);
        }

        if (value is IList<IFormFile> files)
        {
            return AreValidFiles(files);
        }

        if (value is IFormFileCollection fileCollection)
        {
            return AreValidFiles(fileCollection);
        }

        return false;
    }

    private bool AreValidFiles(IEnumerable<IFormFile> files)
        => files.All(IsValidFile);

    private bool IsValidFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return true; // returning false, makes this field required.
        }

        var fileName = file.FileName;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        fileName = fileName.ToLowerInvariant();
        var name = Path.GetFileName(fileName);
        var ext = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return !_extensionsToFilter.Contains(ext, StringComparer.OrdinalIgnoreCase) &&
               !_namesToFilter.Contains(name, StringComparer.OrdinalIgnoreCase) &&
               !_namesToFilter.Contains(ext, StringComparer.OrdinalIgnoreCase) &&

               //for "file.asp;.jpg" files --> run as an ASP file
               _extensionsToFilter.All(item => !name.Contains(item, StringComparison.OrdinalIgnoreCase));
    }
}