using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Persian Date Model Binder
/// </summary>
public class PersianDateModelBinder : IModelBinder
{
    /// <summary>
    ///     Attempts to bind a model.
    /// </summary>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var binderLogger = logger.CreateLogger(nameof(PersianDateModelBinder));

        var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            binderLogger.LogError(message: "There is not valueProvider for bindingContext.ModelName: {ModelName}",
                bindingContext.ModelName);

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
                binderLogger.LogError(message: "{ValueAsString}` is not a valid PersianDateTime.", valueAsString);

                return fallbackBinder.BindModelAsync(bindingContext);
            }

            dt = valueAsString?.ToGregorianDateTime();
        }
        catch (Exception ex)
        {
            var message = $"`{valueProviderResult.FirstValue}` is not a valid Persian date.";
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);

            binderLogger.LogError(ex.Demystify(), message: "PersianDateModelBinder error. {Message}", message);

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
