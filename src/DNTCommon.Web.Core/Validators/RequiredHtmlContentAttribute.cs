namespace DNTCommon.Web.Core;

/// <summary>
///     Determines whether the specified value of the object has an HTML content.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredHtmlContentAttribute : ValidationAttribute
{
    /// <summary>
    ///     Determines whether the specified value of the object has an HTML content.
    /// </summary>
    public override bool IsValid(object? value) => value is not null && !value.ToString().RemoveHtmlTags().IsEmpty();

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(validationContext);

        return string.IsNullOrWhiteSpace(validationContext.MemberName)
            ? new ValidationResult(ErrorMessage)
            : new ValidationResult(ErrorMessage, new[]
            {
                validationContext.MemberName
            });
    }
}