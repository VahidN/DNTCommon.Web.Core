using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Persian Ye Ke Model Binder Provider
    /// </summary>
    public class YeKeModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates an IModelBinder based on ModelBinderProviderContext.
        /// </summary>
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType)
            {
                return null;
            }

            if (context.Metadata.ModelType == typeof(string))
            {
                return new YeKeModelBinder();
            }

            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
        }
    }
}