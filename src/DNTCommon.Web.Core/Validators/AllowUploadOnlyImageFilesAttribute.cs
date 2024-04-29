using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allowing only image files are safe to be uploaded.
///     This validation attribute, checks the content of the uploaded file.
///     More info: http://www.dntips.ir/post/2555
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class AllowUploadOnlyImageFilesAttribute : ValidationAttribute
{
    /// <summary>
    ///     Allowing only image files are safe to be uploaded.
    ///     This validation attribute, checks the content of the uploaded file.
    /// </summary>
    public AllowUploadOnlyImageFilesAttribute()
    {
    }

    /// <summary>
    ///     Allowing only image files are safe to be uploaded.
    ///     This validation attribute, checks the content of the uploaded file.
    /// </summary>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    public AllowUploadOnlyImageFilesAttribute(int maxWidth, int maxHeight)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
    }

    /// <summary>
    ///     maximum allowed width
    /// </summary>
    /// <value></value>
    public int? MaxWidth { get; }

    /// <summary>
    ///     maximum allowed height
    /// </summary>
    /// <value></value>
    public int? MaxHeight { get; }

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
            return IsValidImageFile(file);
        }

        if (value is IList<IFormFile> files)
        {
            return AreValidImageFiles(files);
        }

        if (value is IFormFileCollection fileCollection)
        {
            return AreValidImageFiles(fileCollection);
        }

        return false;
    }

    private bool AreValidImageFiles(IEnumerable<IFormFile> files) => files.All(IsValidImageFile);

    private bool IsValidImageFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return true; // returning false, makes this field required.
        }

        return file.IsValidImageFile(MaxWidth, MaxHeight);
    }
}