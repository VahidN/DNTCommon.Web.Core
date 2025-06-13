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

    /// <summary>
    ///     Allowed files extensions to be uploaded
    /// </summary>
    /// <value></value>
    public string? FileExtensions { get; set; }

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

        if (FileExtensions is null)
        {
            throw new InvalidOperationException($"{nameof(FileExtensions)} is null");
        }

        string[] _allowedExtensions = [.. FileExtensions.Split(Separator, StringSplitOptions.RemoveEmptyEntries)];

        return (
            !string.IsNullOrWhiteSpace(fileExtension) && _allowedExtensions.Any(ext
                => fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase)),
            FileExtensionsErrorMessage ?? ErrorMessage);
    }
}
