using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Extensions for working with filter attributes.
/// </summary>
public static class FilterAttributesExtensions
{
    private static readonly Type StringType = typeof(string);

    /// <summary>
    ///     Cleans all string values in the current ActionArguments and model's string properties using the specified function.
    /// </summary>
    public static void CleanupActionStringValues(this ActionExecutingContext context, Func<string, string> action)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var (aKey, aValue) in context.ActionArguments)
        {
            if (aValue is null)
            {
                continue;
            }

            var type = aValue.GetType();

            if (type == StringType)
            {
                context.ActionArguments[aKey] = action(Convert.ToString(aValue, CultureInfo.InvariantCulture) ?? "");
            }
            else
            {
                var stringProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.CanRead && x.CanWrite && x.PropertyType == StringType &&
                                x.GetGetMethod(nonPublic: true)?.IsPublic == true &&
                                x.GetSetMethod(nonPublic: true)?.IsPublic == true);

                foreach (var propertyInfo in stringProperties)
                {
                    var value = propertyInfo.GetValue(aValue);

                    if (value is null)
                    {
                        continue;
                    }

                    propertyInfo.SetValue(aValue, action(Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""));
                }
            }
        }
    }
}
