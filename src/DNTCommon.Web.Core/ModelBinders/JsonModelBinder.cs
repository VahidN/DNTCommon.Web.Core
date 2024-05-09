using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTCommon.Web.Core;

/// <summary>
///     Json ModelBinder
/// </summary>
public class JsonModelBinder : IModelBinder
{
    /// <summary>
    ///     Attempts to bind a model.
    /// </summary>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None)
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valueAsString = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return Task.CompletedTask;
            }

            var result = JsonSerializer.Deserialize(valueAsString, bindingContext.ModelType);

            if (result is not null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}