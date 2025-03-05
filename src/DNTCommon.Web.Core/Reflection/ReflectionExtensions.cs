using System.ComponentModel;
using System.Text;
using DNTPersianUtils.Core;

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
        where TEnum : struct, Enum
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
    ///     Getting string attribute of Enum's value.
    ///     Processing order is checking DisplayNameAttribute first and then DescriptionAttribute.
    ///     If none of these is available, value.ToString() will be returned.
    /// </summary>
    /// <param name="flags">enum value</param>
    /// <returns>string attribute of Enum's value</returns>
    public static string GetEnumStringValue(this Enum flags)
    {
        ArgumentNullException.ThrowIfNull(flags);

        if (flags.GetType().GetTypeInfo().GetCustomAttributes(inherit: true).OfType<FlagsAttribute>().Any())
        {
            var text = GetEnumFlagsText(flags);

            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
        }

        return GetEnumValueText(flags);
    }

    private static string GetEnumFlagsText(this Enum flags)
    {
        const char leftToRightSeparator = ',';
        const char rightToRightSeparator = '،';

        var sb = new StringBuilder();
        var items = Enum.GetValues(flags.GetType());

        foreach (Enum value in items)
        {
            if (flags.HasFlag(value) && Convert.ToInt64(value, CultureInfo.InvariantCulture) != 0)
            {
                var text = GetEnumValueText(value);
                var separator = text.ContainsFarsi() ? rightToRightSeparator : leftToRightSeparator;
                sb.Append(text).Append(separator).Append(value: ' ');
            }
        }

        return sb.ToString().Trim().TrimEnd(leftToRightSeparator).TrimEnd(rightToRightSeparator);
    }

    private static string GetEnumValueText(this Enum value)
    {
        var text = value.ToString();
        var info = value.GetType().GetField(text);

        var description = info?.GetCustomAttributes(inherit: true).OfType<DescriptionAttribute>().FirstOrDefault();

        if (description is not null)
        {
            return description.Description;
        }

        var displayName = info?.GetCustomAttributes(inherit: true).OfType<DisplayNameAttribute>().FirstOrDefault();

        if (displayName is not null)
        {
            return displayName.DisplayName;
        }

        var display = info?.GetCustomAttributes(inherit: true).OfType<DisplayAttribute>().FirstOrDefault();

        if (display is not null)
        {
            return display.Name ?? "";
        }

        return text;
    }

    /// <summary>
    ///     Gets the actual enum type. It unwrap Nullable of T if needed
    /// </summary>
    public static Type GetEnumType<TEnum>()
        where TEnum : struct, Enum
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));

        return nullableType ?? typeof(TEnum);
    }
}
