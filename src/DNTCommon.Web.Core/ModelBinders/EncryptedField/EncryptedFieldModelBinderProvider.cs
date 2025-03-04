﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace DNTCommon.Web.Core;

/// <summary>
///     EncryptedField ModelBinder Provider
/// </summary>
public class EncryptedFieldModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    ///     Creates an IModelBinder based on ModelBinderProviderContext.
    /// </summary>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.IsComplexType)
        {
            return null;
        }

        var propName = context.Metadata.PropertyName;

        if (string.IsNullOrWhiteSpace(propName))
        {
            return null;
        }

        var propInfo = context.Metadata.ContainerType?.GetProperty(propName);

        if (propInfo is null)
        {
            return null;
        }

        var attribute = propInfo.GetCustomAttributes(typeof(EncryptedFieldAttribute), inherit: false).FirstOrDefault();

        if (attribute is null)
        {
            return null;
        }

        return new BinderTypeModelBinder(typeof(EncryptedFieldModelBinder));
    }
}
