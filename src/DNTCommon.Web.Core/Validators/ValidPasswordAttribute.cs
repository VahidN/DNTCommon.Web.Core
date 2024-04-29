using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Determines whether the specified value of the object is a valid password.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ValidPasswordAttribute : ValidationAttribute
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

    private static readonly Regex Lowercase = new("[a-z]+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Uppercase = new("[A-Z]+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Digit = new("(\\d)+", RegexOptions.Compiled, Timeout);
    private static readonly Regex Symbol = new("(\\W)+", RegexOptions.Compiled, Timeout);

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveLowercaseLetters { set; get; } = true;

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveUppercaseLetters { set; get; } = true;

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveDigits { set; get; } = true;

    /// <summary>
    ///     Its default value is true
    /// </summary>
    public bool ShouldHaveSymbols { set; get; } = true;

    /// <summary>
    ///     Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true; // returning false, makes this field required.
        }

        var valStr = value.ToString();

        if (string.IsNullOrWhiteSpace(valStr))
        {
            return true; // returning false, makes this field required.
        }

        return HasValidPassword(valStr);
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
            : new ValidationResult(ErrorMessage, new[]
            {
                validationContext.MemberName
            });
    }

    private bool HasValidPassword(string password)
    {
        if (ShouldHaveLowercaseLetters && !Lowercase.IsMatch(password))
        {
            return false;
        }

        if (ShouldHaveUppercaseLetters && !Uppercase.IsMatch(password))
        {
            return false;
        }

        if (ShouldHaveDigits && !Digit.IsMatch(password))
        {
            return false;
        }

        return ShouldHaveSymbols && Symbol.IsMatch(password);
    }
}