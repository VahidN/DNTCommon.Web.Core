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
    ///     maximum allowed width.
    ///     It will be ignored if it's 0.
    /// </summary>
    /// <value></value>
    public int MaxWidth { get; set; }

    /// <summary>
    ///     maximum allowed height.
    ///     It will be ignored if it's 0.
    /// </summary>
    /// <value></value>
    public int MaxHeight { get; set; }

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

        if (MaxFileSizeInBytes > 0 && file.Length > MaxFileSizeInBytes)
        {
            return (false, MaxFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        if (MinFileSizeInBytes > 0 && file.Length < MinFileSizeInBytes)
        {
            return (false, MinFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        return (file.IsValidImageFile(MaxWidth, MaxHeight), MaxWidthMaxHeightErrorMessage ?? ErrorMessage);
    }
}
