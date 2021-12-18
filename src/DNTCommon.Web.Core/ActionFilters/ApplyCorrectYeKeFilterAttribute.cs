using System;
using System.Linq;
using System.Reflection;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core
{
    /// <summary>
    ///     Fixes common writing mistakes caused by using a bad keyboard layout, such as replacing Arabic Ye with Persian one
    ///     and so on ...
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ApplyCorrectYeKeFilterAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext? context)
        {
            if (context is null)
            {
                return;
            }

            foreach (var (aKey, aValue) in context.ActionArguments)
            {
                if (aValue is null)
                {
                    continue;
                }

                var type = aValue.GetType();
                if (type == typeof(string))
                {
                    context.ActionArguments[aKey] = aValue.ToString().ApplyCorrectYeKe();
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

                        propertyInfo.SetValue(aValue, value.ToString().ApplyCorrectYeKe());
                    }
                }
            }
        }
    }
}