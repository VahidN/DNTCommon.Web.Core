using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Determines whether the specified value of the object is a valid password.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ValidPasswordAttribute : ValidationAttribute
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(value: 1);

    private static readonly Regex Lowercase = new(pattern: "[a-z]+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Uppercase = new(pattern: "[A-Z]+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Digit = new(pattern: "(\\d)+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Symbol = new(pattern: "(\\W)+", RegexOptions.Compiled, Timeout);

    /// <summary>
    ///     Should user provide a non-null value for this field?
    /// </summary>
    public bool IsRequired { set; get; }

    /// <summary>
    ///     A custom error message for IsRequired
    /// </summary>
    public string? IsRequiredErrorMessage { set; get; }

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveLowercaseLetters { set; get; } = true;

    /// <summary>
    ///     A custom error message for ShouldHaveLowercaseLetters
    /// </summary>
    public string? ShouldHaveLowercaseLettersErrorMessage { set; get; }

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveUppercaseLetters { set; get; } = true;

    /// <summary>
    ///     A custom error message for ShouldHaveUppercaseLetters
    /// </summary>
    public string? ShouldHaveUppercaseLettersErrorMessage { set; get; }

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveDigits { set; get; } = true;

    /// <summary>
    ///     A custom error message for ShouldHaveDigits
    /// </summary>
    public string? ShouldHaveDigitsErrorMessage { set; get; }

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveSymbols { set; get; } = true;

    /// <summary>
    ///     A custom error message for ShouldHaveSymbols
    /// </summary>
    public string? ShouldHaveSymbolsErrorMessage { set; get; }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        var (success, _) = IsValidPassword(value);

        return success;
    }

    private (bool Success, string? ErrorMessage) IsValidPassword(object? value)
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

        return HasValidPassword(valStr);
    }

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var (success, errorMessage) = IsValidPassword(value);

        if (success)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(validationContext);

        return string.IsNullOrWhiteSpace(validationContext.MemberName)
            ? new ValidationResult(errorMessage)
            : new ValidationResult(errorMessage, [validationContext.MemberName]);
    }

    private (bool Success, string? ErrorMessage) HasValidPassword(string password)
    {
        if (ShouldHaveLowercaseLetters && !Lowercase.IsMatch(password))
        {
            return (false, ShouldHaveLowercaseLettersErrorMessage ?? ErrorMessage);
        }

        if (ShouldHaveUppercaseLetters && !Uppercase.IsMatch(password))
        {
            return (false, ShouldHaveUppercaseLettersErrorMessage ?? ErrorMessage);
        }

        if (ShouldHaveDigits && !Digit.IsMatch(password))
        {
            return (false, ShouldHaveDigitsErrorMessage ?? ErrorMessage);
        }

        if (ShouldHaveSymbols && !Symbol.IsMatch(password))
        {
            return (false, ShouldHaveSymbolsErrorMessage ?? ErrorMessage);
        }

        return (true, null);
    }
}
