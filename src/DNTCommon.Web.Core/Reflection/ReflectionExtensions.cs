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
    public static IReadOnlyDictionary<string, TEnum> GetEnumItems<TEnum>()
        where TEnum : Enum
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
    ///     Gets the actual enum type. It unwrap Nullable of T if needed
    /// </summary>
    public static Type GetEnumType<TEnum>()
        where TEnum : Enum
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));

        return nullableType ?? typeof(TEnum);
    }
}