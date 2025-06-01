using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allowing only selected file extensions are safe to be uploaded.
///     More info: http://www.dntips.ir/post/2555
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class UploadFileExtensionsAttribute : UploadFileValidationBaseAttribute
{
    private static readonly char[] Separator = [','];

    private readonly IList<string> _allowedExtensions;

    /// <summary>
    ///     Allowing only selected file extensions are safe to be uploaded.
    /// </summary>
    /// <param name="fileExtensions">Allowed files extensions to be uploaded</param>
    /// <param name="allowUploadEmptyFiles">Determines whether empty files can be uploaded</param>
    /// <param name="maxFileSizeInBytes">Max allowed file size. It will be ignored if it's null.</param>
    /// <param name="minFileSizeInBytes">Min allowed file size. It will be ignored if it's null.</param>
    public UploadFileExtensionsAttribute(string fileExtensions,
        bool allowUploadEmptyFiles = false,
        long? maxFileSizeInBytes = null,
        long? minFileSizeInBytes = null)
    {
        if (string.IsNullOrWhiteSpace(fileExtensions))
        {
            throw new ArgumentNullException(nameof(fileExtensions));
        }

        AllowUploadEmptyFiles = allowUploadEmptyFiles;
        MaxFileSizeInBytes = maxFileSizeInBytes;
        MinFileSizeInBytes = minFileSizeInBytes;

        FileExtensions = fileExtensions;
        _allowedExtensions = [.. fileExtensions.Split(Separator, StringSplitOptions.RemoveEmptyEntries)];
    }

    /// <summary>
    ///     Allowed files extensions to be uploaded
    /// </summary>
    /// <value></value>
    public string FileExtensions { get; }

    /// <summary>
    ///     A custom error message for FileExtensions
    /// </summary>
    public string? FileExtensionsErrorMessage { set; get; }

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

        var fileExtension = Path.GetExtension(file.FileName);

        return (
            !string.IsNullOrWhiteSpace(fileExtension) &&
            _allowedExtensions.Any(ext => fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase)),
            FileExtensionsErrorMessage ?? ErrorMessage);
    }
}
