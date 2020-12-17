using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Persian Date Model Binder Extensions
    /// </summary>
    public static class PersianDateModelBinderExtensions
    {
        /// <summary>
        /// Inserts PersianDateModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
        /// </summary>
        public static MvcOptions UsePersianDateModelBinder(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0, new PersianDateModelBinderProvider());
            return options;
        }
    }

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

    /// <summary>
    /// Persian Date Model Binder
    /// </summary>
    public class PersianDateModelBinder : IModelBinder
    {
        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1031:catch a more specific allowed exception type, or rethrow the exception",
                Justification = "The exception will be logged.")]
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            DateTime? dt;
            try
            {
                var valueAsString = valueProviderResult.FirstValue;
                var isValidPersianDateTime = !string.IsNullOrWhiteSpace(valueAsString) &&
                                             valueAsString.IsValidPersianDateTime();

                if (!isValidPersianDateTime)
                {
                    return fallbackBinder.BindModelAsync(bindingContext);
                }

                dt = valueAsString?.ToGregorianDateTime();
            }
            catch (Exception ex)
            {
                var message = $"`{valueProviderResult.FirstValue}` is not a valid Persian date.";
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);

                var binderLogger = logger.CreateLogger(nameof(PersianDateModelBinder));
                binderLogger.LogError("PersianDateModelBinder error", ex, message);

                return Task.CompletedTask;
            }

            if (Nullable.GetUnderlyingType(bindingContext.ModelType) == typeof(DateTime))
            {
                bindingContext.Result = ModelBindingResult.Success(dt);
            }
            else if (dt.HasValue)
            {
                bindingContext.Result = ModelBindingResult.Success(dt.Value);
            }
            return Task.CompletedTask;
        }
    }
}