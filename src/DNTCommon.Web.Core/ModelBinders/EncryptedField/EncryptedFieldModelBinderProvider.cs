using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Linq;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Ye EncryptedField Model Binder Extensions
    /// </summary>
    public static class EncryptedFieldModelBinderExtensions
    {
        /// <summary>
        /// Inserts EncryptedFieldModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
        /// </summary>
        public static MvcOptions UseEncryptedFieldModelBinder(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0, new EncryptedFieldModelBinderProvider());
            return options;
        }
    }

    /// <summary>
    /// EncryptedField ModelBinder Provider
    /// </summary>
    public class EncryptedFieldModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates an IModelBinder based on ModelBinderProviderContext.
        /// </summary>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType)
            {
                return null;
            }

            var propName = context.Metadata.PropertyName;
            if (string.IsNullOrWhiteSpace(propName))
            {
                return null;
            }

            var propInfo = context.Metadata.ContainerType.GetProperty(propName);
            if (propInfo == null)
            {
                return null;
            }

            var attribute = propInfo.GetCustomAttributes(typeof(EncryptedFieldAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }

            return new BinderTypeModelBinder(typeof(EncryptedFieldModelBinder));
        }
    }
}