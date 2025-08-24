using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     A base class for UploadFile Validations
/// </summary>
public abstract class UploadFileValidationBaseAttribute : ValidationAttribute
{
    /// <summary>
    ///     Should user provide a non-null value for this field?
    /// </summary>
    public bool IsRequired { set; get; }

    /// <summary>
    ///     A custom error message for IsRequired
    /// </summary>
    public string? IsRequiredErrorMessage { set; get; }

    /// <summary>
    ///     Max allowed file size. It will be ignored if it's 0.
    /// </summary>
    public long MaxFileSizeInBytes { get; set; }

    /// <summary>
    ///     A custom error message for MaxFileSizeInBytes
    /// </summary>
    public string? MaxFileSizeInBytesErrorMessage { set; get; }

    /// <summary>
    ///     Min allowed file size. It will be ignored if it's 0.
    /// </summary>
    public long MinFileSizeInBytes { get; set; }

    /// <summary>
    ///     A custom error message for MinFileSizeInBytes
    /// </summary>
    public string? MinFileSizeInBytesErrorMessage { set; get; }

    /// <summary>
    ///     Validates the input file
    /// </summary>
    public abstract (bool Success, string? ErrorMessage) IsValidFile(IFormFile? file);

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        var (success, _) = value.IsValidIFormFile(IsValidFile, ErrorMessage);

        return success;
    }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var (success, errorMessage) = value.IsValidIFormFile(IsValidFile, ErrorMessage);

        if (success)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(validationContext);

        return string.IsNullOrWhiteSpace(validationContext.MemberName)
            ? new ValidationResult(errorMessage)
            : new ValidationResult(errorMessage, [validationContext.MemberName]);
    }

    protected (bool Success, string? ErrorMessage) HasValidFileSize(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return (!IsRequired, IsRequiredErrorMessage ?? ErrorMessage);
        }

        if (MaxFileSizeInBytes > 0 && file.Length > MaxFileSizeInBytes)
        {
            return (false, MaxFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        if (MinFileSizeInBytes > 0 && file.Length < MinFileSizeInBytes)
        {
            return (false, MinFileSizeInBytesErrorMessage ?? ErrorMessage);
        }

        return (true, null);
    }
}
