using System.Collections;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
/// </summary>
/// <remarks>
///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class EncryptedFieldResultFilterAttribute(
    IProtectionProviderService protectionProviderService,
    ILogger<EncryptedFieldResultFilterAttribute> logger) : ResultFilterAttribute
{
    private readonly ILogger<EncryptedFieldResultFilterAttribute> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly ConcurrentDictionary<Type, bool> _modelsWithEncryptedFieldAttributes = new();

    private readonly IProtectionProviderService _protectionProviderService = protectionProviderService ??
                                                                             throw new ArgumentNullException(
                                                                                 nameof(protectionProviderService));

    /// <summary>
    ///     An IProtectionProvider
    /// </summary>
    /// <value></value>
    public IProtectionProviderService ProtectionProviderService { get; } = protectionProviderService;

    /// <summary>
    ///     The current ILogger
    /// </summary>
    /// <value></value>
    public ILogger<EncryptedFieldResultFilterAttribute> Logger { get; } = logger;

    /// <summary>
    ///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
    /// </summary>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var model = context.Result switch
        {
            PageResult pageResult => pageResult.Model, // For Razor pages
            ViewResult viewResult => viewResult.Model, // For MVC Views
            PartialViewResult partialViewResult => partialViewResult.Model, // For `return PartialView`
            ObjectResult objectResult => objectResult.Value, // For Web API results
            _ => null
        };

        if (model is null)
        {
            return;
        }

        if (model is IEnumerable)
        {
            if (model is IEnumerable items)
            {
                foreach (var item in items)
                {
                    EncryptProperties(item);
                }
            }
        }
        else
        {
            EncryptProperties(model);
        }
    }

    private void EncryptProperties(object? model)
    {
        if (model is null)
        {
            return;
        }

        var modelType = model.GetType();

        if (_modelsWithEncryptedFieldAttributes.TryGetValue(modelType, out var hasEncryptedFieldAttribute) &&
            !hasEncryptedFieldAttribute)
        {
            return;
        }

        foreach (var property in modelType.GetProperties())
        {
            var attribute = property.GetCustomAttributes(typeof(EncryptedFieldAttribute), inherit: false)
                .FirstOrDefault();

            if (attribute is null)
            {
                continue;
            }

            hasEncryptedFieldAttribute = true;

            var value = property.GetValue(model);

            if (value is null)
            {
                continue;
            }

            if (value is not string)
            {
                _logger.LogWarning(
                    message:
                    "[EncryptedField] should be applied to `string` properties, But type of `{PropertyDeclaringType}.{PropertyName}` is `{PropertyPropertyType}`.",
                    property.DeclaringType, property.Name, property.PropertyType);

                continue;
            }

            var inputText = value.ToInvariantString();
            var encryptedData = _protectionProviderService.Encrypt(inputText);
            property.SetValue(model, encryptedData);
        }

        _modelsWithEncryptedFieldAttributes.TryAdd(modelType, hasEncryptedFieldAttribute);
    }
}
