namespace DNTCommon.Web.Core;

/// <summary>
///     Determines whether the specified value of the object has an HTML content.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredHtmlContentAttribute : ValidationAttribute
{
    /// <summary>
    ///     Should user provide a non-null value for this field?
    ///     Its default value is true.
    /// </summary>
    public bool IsRequired { set; get; } = true;

    /// <summary>
    ///     A custom error message for IsRequired
    /// </summary>
    public string? IsRequiredErrorMessage { set; get; }

    /// <summary>
    ///     Determines whether the specified value of the object has an HTML content.
    /// </summary>
    public override bool IsValid(object? value)
    {
        var (success, _) = IsValidContent(value);

        return success;
    }

    private (bool Success, string? ErrorMessage) IsValidContent(object? value)
    {
        if (value is null)
        {
            return (!IsRequired, IsRequiredErrorMessage ?? ErrorMessage);
        }

        var valStr = value.ToInvariantString();

        if (string.IsNullOrWhiteSpace(valStr))
        {
            return (!IsRequired, IsRequiredErrorMessage ?? ErrorMessage);
        }

        return (!valStr.RemoveHtmlTags().IsEmpty(), ErrorMessage);
    }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var (success, errorMessage) = IsValidContent(value);

        if (success)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(validationContext);

        return string.IsNullOrWhiteSpace(validationContext.MemberName)
            ? new ValidationResult(errorMessage)
            : new ValidationResult(errorMessage, [validationContext.MemberName]);
    }
}
