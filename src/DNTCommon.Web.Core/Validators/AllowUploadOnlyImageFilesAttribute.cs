using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allowing only image files are safe to be uploaded.
///     This validation attribute, checks the content of the uploaded file.
///     More info: http://www.dntips.ir/post/2555
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class AllowUploadOnlyImageFilesAttribute : UploadFileValidationBaseAttribute
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
    ///     A custom error message for MaxWidthMaxHeight
    /// </summary>
    public string? MaxWidthMaxHeightErrorMessage { set; get; }

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

        return (file.IsValidImageFile(MaxWidth, MaxHeight), MaxWidthMaxHeightErrorMessage ?? ErrorMessage);
    }
}
