using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allowing only selected file extensions are safe to be uploaded.
///     More info: http://www.dntips.ir/post/2555
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class UploadFileExtensionsAttribute : ValidationAttribute
{
    private static readonly char[] Separator = [','];

    private readonly IList<string> _allowedExtensions;

    /// <summary>
    ///     Allowing only selected file extensions are safe to be uploaded.
    /// </summary>
    /// <param name="fileExtensions">Allowed files extensions to be uploaded</param>
    public UploadFileExtensionsAttribute(string fileExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileExtensions))
        {
            throw new ArgumentNullException(nameof(fileExtensions));
        }

        FileExtensions = fileExtensions;
        _allowedExtensions = [.. fileExtensions.Split(Separator, StringSplitOptions.RemoveEmptyEntries)];
    }

    /// <summary>
    ///     Allowed files extensions to be uploaded
    /// </summary>
    /// <value></value>
    public string FileExtensions { get; }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value is null)
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

    private bool AreValidFiles(IEnumerable<IFormFile> files) => files.All(IsValidFile);

    private bool IsValidFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return true; // returning false, makes this field required.
        }

        var fileExtension = Path.GetExtension(file.FileName);

        return !string.IsNullOrWhiteSpace(fileExtension) &&
               _allowedExtensions.Any(ext => fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }
}
