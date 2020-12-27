using Microsoft.AspNetCore.Mvc;
using System;

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
}