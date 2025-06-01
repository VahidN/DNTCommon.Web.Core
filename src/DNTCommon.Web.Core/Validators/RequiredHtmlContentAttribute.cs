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
    ///     Determines whether the specified value of the object has an HTML content.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return !IsRequired;
        }

        var valStr = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(valStr))
        {
            return !IsRequired;
        }

        return !valStr.RemoveHtmlTags().IsEmpty();
    }

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
            : new ValidationResult(ErrorMessage, [validationContext.MemberName]);
    }
}
