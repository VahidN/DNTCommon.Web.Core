using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Extensions for working with filter attributes.
/// </summary>
public static class FilterAttributesExtensions
{
    private static readonly Type StringType = typeof(string);

    /// <summary>
    ///     Cleans all string values in the current ActionArguments, including collections of strings and model's string
    ///     properties, using the specified function.
    /// </summary>
    public static void CleanupActionStringValues(this ActionExecutingContext context, Func<string, string> action)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var (key, argumentValue) in context.ActionArguments)
        {
            switch (argumentValue)
            {
                case null:
                    continue;

                // Case 1: The argument is a simple string.
                case string strValue:
                    context.ActionArguments[key] = action(strValue);

                    break;

                // Case 2: The argument is a list of strings.
                case List<string> stringList:
                    for (var i = 0; i < stringList.Count; i++)
                    {
                        stringList[i] = action(stringList[i]);
                    }

                    break;

                // Case 3: The argument is an array of strings.
                case string[] stringArray:
                    for (var i = 0; i < stringArray.Length; i++)
                    {
                        stringArray[i] = action(stringArray[i]);
                    }

                    break;

                // Case 4: The argument is a jagged array of strings (array of string arrays).
                case string[][] jaggedStringArray:
                    foreach (var innerArray in jaggedStringArray)
                    {
                        if (innerArray is null)
                        {
                            continue;
                        }

                        for (var j = 0; j < innerArray.Length; j++)
                        {
                            innerArray[j] = action(innerArray[j]);
                        }
                    }

                    break;

                // Case 5: The argument is a complex object.
                // This part remains the same to handle string properties of a model.
                default:
                    foreach (var stringProperty in argumentValue.GetType()
                                 .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(static p
                                     => p is { CanRead: true, CanWrite: true } && p.PropertyType == StringType &&
                                        p.GetGetMethod(nonPublic: true)?.IsPublic == true &&
                                        p.GetSetMethod(nonPublic: true)?.IsPublic == true))
                    {
                        var value = stringProperty.GetValue(argumentValue);

                        if (value is string propValue)
                        {
                            stringProperty.SetValue(argumentValue, action(propValue));
                        }
                    }

                    break;
            }
        }
    }
}
