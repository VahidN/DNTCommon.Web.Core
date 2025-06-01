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
    /// <param name="allowUploadEmptyFiles">Determines whether empty files can be uploaded</param>
    /// <param name="maxFileSizeInBytes">Max allowed file size. It will be ignored if it's null.</param>
    /// <param name="minFileSizeInBytes">Min allowed file size. It will be ignored if it's null.</param>
    public AllowUploadOnlyImageFilesAttribute(int maxWidth,
        int maxHeight,
        bool allowUploadEmptyFiles = false,
        long? maxFileSizeInBytes = null,
        long? minFileSizeInBytes = null)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        AllowUploadEmptyFiles = allowUploadEmptyFiles;
        MaxFileSizeInBytes = maxFileSizeInBytes;
        MinFileSizeInBytes = minFileSizeInBytes;
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
    ///     Determines whether empty files can be uploaded
    /// </summary>
    public bool AllowUploadEmptyFiles { get; }

    /// <summary>
    ///     Should user provide a non-null value for this field?
    /// </summary>
    public bool IsRequired { set; get; }

    /// <summary>
    ///     Max allowed file size. It will be ignored if it's null.
    /// </summary>
    public long? MaxFileSizeInBytes { get; }

    /// <summary>
    ///     Min allowed file size. It will be ignored if it's null.
    /// </summary>
    public long? MinFileSizeInBytes { get; }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return !IsRequired;
        }

        return value.IsValidIFormFile(IsValidImageFile);
    }

    private bool IsValidImageFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return AllowUploadEmptyFiles;
        }

        if (MaxFileSizeInBytes.HasValue && file.Length > MaxFileSizeInBytes.Value)
        {
            return false;
        }

        if (MinFileSizeInBytes.HasValue && file.Length < MinFileSizeInBytes.Value)
        {
            return false;
        }

        return file.IsValidImageFile(MaxWidth, MaxHeight);
    }
}
