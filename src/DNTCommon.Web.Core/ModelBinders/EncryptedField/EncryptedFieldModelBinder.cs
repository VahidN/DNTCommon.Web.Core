using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
    /// </summary>
    public class EncryptedFieldModelBinder : IModelBinder
    {
        private readonly IProtectionProviderService _protectionProviderService;

        /// <summary>
        /// EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
        /// </summary>
        public EncryptedFieldModelBinder(IProtectionProviderService protectionProviderService)
        {
            _protectionProviderService = protectionProviderService;
        }

        /// <summary>
        /// EncryptedFieldModelBinder decrypts the received encrypted models from the clients.
        /// </summary>
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

            var valueAsString = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            var decryptedResult = _protectionProviderService.Decrypt(valueAsString);
            bindingContext.Result = ModelBindingResult.Success(decryptedResult);
            return Task.CompletedTask;
        }
    }
}