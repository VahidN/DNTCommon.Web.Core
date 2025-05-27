namespace DNTCommon.Web.Core;

/// <summary>
///     A detector for hidden, typographic, and ideographic variation selector (IVS) Unicode characters
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ShouldContainOnlyVisibleCharactersAttribute : ValidationAttribute
{
    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true; // returning false, makes this field required.
        }

        var valStr = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(valStr))
        {
            return true; // returning false, makes this field required.
        }

        return !valStr.HasHiddenCharacters();
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
