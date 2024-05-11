namespace DNTCommon.Web.Core;

/// <summary>
///     Reflection Extensions
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    ///     Gets the list of defined enum items.
    ///     It uses the DisplayAttribute if set on the enum member, so this support localization.
    /// </summary>
    public static IReadOnlyDictionary<string, TEnum> GetEnumItems<TEnum>() where TEnum : System.Enum
    {
        var results = new Dictionary<string, TEnum>(StringComparer.Ordinal);
        var enumType = GetEnumType<TEnum>();
        foreach (TEnum value in Enum.GetValues(enumType))
        {
            results.Add(value.GetDisplayName(), value);
        }

        return results;
    }

    /// <summary>
    ///     Gets the display text for an enum value.
    ///     Uses the DisplayAttribute if set on the enum member, so this support localization.
    /// </summary>
    public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : System.Enum
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var name = value.ToString();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(value));
        }

        var member = value.GetType().GetMember(name)[0];
        var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.GetName() ?? name;
    }

    /// <summary>
    ///     Gets the actual enum type. It unwrap Nullable of T if needed
    /// </summary>
    public static Type GetEnumType<TEnum>() where TEnum : System.Enum
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));
        return nullableType ?? typeof(TEnum);
    }
}