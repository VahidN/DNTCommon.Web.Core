using System.ComponentModel;

namespace DNTCommon.Web.Core;

/// <summary>
///     Property Extensions
/// </summary>
public static class PropertyExtensions
{
    /// <summary>
    ///     Gets value of a property, including enum's string description and NullDisplayTextAttribute.
    /// </summary>
    /// <param name="propertyInfo">property info</param>
    /// <param name="instance">object's instance</param>
    /// <returns>value of the property</returns>
    public static object? GetPropertyValue(this PropertyInfo propertyInfo, object instance)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var value = propertyInfo.GetValue(instance, index: null);

        if (value is not null && propertyInfo.PropertyType.GetTypeInfo().IsEnum)
        {
            return ((Enum)value).GetEnumStringValue();
        }

        if (value is null)
        {
            var nullDisplayText = propertyInfo.GetNullDisplayTextAttribute();

            if (!string.IsNullOrEmpty(nullDisplayText))
            {
                return nullDisplayText;
            }
        }

        return value;
    }

    /// <summary>
    ///     Returns DisplayFormatAttribute data.
    /// </summary>
    /// <param name="info">Property metadata info</param>
    /// <returns>NullDisplayText</returns>
    public static string? GetNullDisplayTextAttribute(this MemberInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        var displayFormat = info.GetCustomAttributes(inherit: true).OfType<DisplayFormatAttribute>().FirstOrDefault();

        return displayFormat is null ? string.Empty : displayFormat.NullDisplayText;
    }

    /// <summary>
    ///     Returns DisplayFormatAttribute data.
    /// </summary>
    /// <param name="info">Property metadata info</param>
    /// <returns>DataFormatString</returns>
    public static string? GetDataFormatStringAttribute(this MemberInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        var displayFormat = info.GetCustomAttributes(inherit: true).OfType<DisplayFormatAttribute>().FirstOrDefault();

        return displayFormat is null ? string.Empty : displayFormat.DataFormatString;
    }

    /// <summary>
    ///     Returns DisplayNameAttribute data.
    ///     Processing order is checking DisplayNameAttribute and finally DescriptionAttribute.
    ///     If none of these is available, the actual property name will be returned.
    /// </summary>
    /// <param name="info">Property metadata info</param>
    /// <returns>PropertyName</returns>
    public static string? GetPropertyNameAttribute(this MemberInfo? info)
    {
        var displayName = info?.GetCustomAttributes(inherit: true).OfType<DisplayNameAttribute>().FirstOrDefault();

        if (displayName is not null)
        {
            return displayName.DisplayName;
        }

        var description = info?.GetCustomAttributes(inherit: true).OfType<DescriptionAttribute>().FirstOrDefault();

        if (description is not null)
        {
            return description.Description;
        }

        var display = info?.GetCustomAttributes(inherit: true).OfType<DisplayAttribute>().FirstOrDefault();

        if (display is not null)
        {
            return display.Name;
        }

        return null;
    }
}
