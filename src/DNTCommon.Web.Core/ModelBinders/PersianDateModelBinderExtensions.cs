using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     Persian Date Model Binder Extensions
/// </summary>
public static class PersianDateModelBinderExtensions
{
    /// <summary>
    ///     Inserts PersianDateModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
    /// </summary>
    public static MvcOptions UsePersianDateModelBinder(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ModelBinderProviders.Insert(index: 0, new PersianDateModelBinderProvider());

        return options;
    }
}
