using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
/// </summary>
/// <remarks>
///     EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
/// </remarks>
public class EncryptedFieldModelBinder(IProtectionProviderService protectionProviderService) : IModelBinder
{
    /// <summary>
    ///     EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
    /// </summary>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return fallbackBinder.BindModelAsync(bindingContext);
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var valueAsString = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(valueAsString))
        {
            return fallbackBinder.BindModelAsync(bindingContext);
        }

        var decryptedResult = protectionProviderService.Decrypt(valueAsString);
        bindingContext.Result = ModelBindingResult.Success(decryptedResult);

        return Task.CompletedTask;
    }
}
