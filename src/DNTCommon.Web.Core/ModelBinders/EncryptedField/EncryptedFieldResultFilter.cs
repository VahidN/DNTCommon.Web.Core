using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// This filter encrypts the outgoing Models of action methods, before sending them to the client.
    /// </summary>
    public class EncryptedFieldResultFilter : ResultFilterAttribute
    {
        private readonly IProtectionProviderService _protectionProviderService;
        private readonly ILogger<EncryptedFieldResultFilter> _logger;
        private readonly ConcurrentDictionary<Type, bool> _modelsWithEncryptedFieldAttributes = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// This filter encrypts the outgoing Models of action methods, before sending them to the client.
        /// </summary>
        public EncryptedFieldResultFilter(
            IProtectionProviderService protectionProviderService,
            ILogger<EncryptedFieldResultFilter> logger)
        {
            _protectionProviderService = protectionProviderService;
            _logger = logger;
        }

        /// <summary>
        /// This filter encrypts the outgoing Models of action methods, before sending them to the client.
        /// </summary>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var model = context.Result switch
            {
                PageResult pageResult => pageResult.Model, // For Razor pages
                ViewResult viewResult => viewResult.Model, // For MVC Views
                ObjectResult objectResult => objectResult.Value, // For Web API results
                _ => null
            };

            if (model is null)
            {
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(model.GetType()))
            {
                foreach (var item in model as IEnumerable)
                {
                    encryptProperties(item);
                }
            }
            else
            {
                encryptProperties(model);
            }
        }

        private void encryptProperties(object model)
        {
            var modelType = model.GetType();
            if (_modelsWithEncryptedFieldAttributes.TryGetValue(modelType, out var hasEncryptedFieldAttribute)
                && !hasEncryptedFieldAttribute)
            {
                return;
            }

            foreach (var property in modelType.GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(EncryptedFieldAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                hasEncryptedFieldAttribute = true;

                var value = property.GetValue(model);
                if (value is null)
                {
                    continue;
                }

                if (value.GetType() != typeof(string))
                {
                    _logger.LogWarning($"[EncryptedField] should be applied to `string` proprties, But type of `{property.DeclaringType}.{property.Name}` is `{property.PropertyType}`.");
                    continue;
                }

                var encryptedData = _protectionProviderService.Encrypt(value.ToString());
                property.SetValue(model, encryptedData);
            }

            _modelsWithEncryptedFieldAttributes.TryAdd(modelType, hasEncryptedFieldAttribute);
        }
    }
}