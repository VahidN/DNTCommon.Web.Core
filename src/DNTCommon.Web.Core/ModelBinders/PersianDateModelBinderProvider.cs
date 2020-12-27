using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Persian Date Model Binder Provider
    /// </summary>
    public class PersianDateModelBinderProvider : IModelBinderProvider
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

            if (context.Metadata?.IsComplexType != false)
            {
                return null;
            }

            if (context.Metadata.ModelType == typeof(DateTime?) ||
                context.Metadata.ModelType == typeof(DateTime) ||
                context.Metadata.ModelType == typeof(DateTimeOffset) ||
                context.Metadata.ModelType == typeof(DateTimeOffset?))
            {
                return new PersianDateModelBinder();
            }

            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
        }
    }
}