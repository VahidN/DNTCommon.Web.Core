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
#pragma warning disable CC001,MA0137
    /// <summary>
    ///     Attempts to bind a model.
    /// </summary>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
#pragma warning restore CC001, MA0137
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

            await fallbackBinder.BindModelAsync(bindingContext);

            return;
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

                await fallbackBinder.BindModelAsync(bindingContext);

                return;
            }

            dt = valueAsString?.ToGregorianDateTime();
        }
        catch (Exception ex)
        {
            var message = $"`{valueProviderResult.FirstValue}` is not a valid Persian date.";
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);

            binderLogger.LogError(ex.Demystify(), message: "PersianDateModelBinder error. {Message}", message);

            await Task.CompletedTask;

            return;
        }

        if (Nullable.GetUnderlyingType(bindingContext.ModelType) == typeof(DateTime))
        {
            bindingContext.Result = ModelBindingResult.Success(dt);
        }
        else if (dt.HasValue)
        {
            bindingContext.Result = ModelBindingResult.Success(dt.Value);
        }

        await Task.CompletedTask;
    }
}
