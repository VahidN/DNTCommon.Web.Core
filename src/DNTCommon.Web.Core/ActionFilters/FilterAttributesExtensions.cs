using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     FilterAttributes Extensions
/// </summary>
public static class FilterAttributesExtensions
{
    /// <summary>
    ///     Cleans all of the string values of the current ActionArguments and model's stringProperties
    /// </summary>
    public static void CleanupActionStringValues(this ActionExecutingContext context, Func<string, string> action)
    {
        foreach (var (aKey, aValue) in context.ActionArguments)
        {
            if (aValue is null)
            {
                continue;
            }

            var type = aValue.GetType();
            if (type == typeof(string))
            {
                context.ActionArguments[aKey] = action(aValue.ToString() ?? "");
            }
            else
            {
                var stringProperties = aValue.GetType()
                                             .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(x =>
                                                        x.CanRead
                                                        && x.CanWrite
                                                        && x.PropertyType == typeof(string)
                                                        && x.GetGetMethod(true)?.IsPublic == true
                                                        && x.GetSetMethod(true)?.IsPublic == true);

                foreach (var propertyInfo in stringProperties)
                {
                    var value = propertyInfo.GetValue(aValue);
                    if (value is null)
                    {
                        continue;
                    }

                    propertyInfo.SetValue(aValue, action(value.ToString() ?? ""));
                }
            }
        }
    }
}