﻿using System.Collections;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
/// </summary>
public sealed class EncryptedFieldResultFilterAttribute : ResultFilterAttribute
{
    private readonly ILogger<EncryptedFieldResultFilterAttribute> _logger;
    private readonly ConcurrentDictionary<Type, bool> _modelsWithEncryptedFieldAttributes = new();
    private readonly IProtectionProviderService _protectionProviderService;

    /// <summary>
    ///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
    /// </summary>
    public EncryptedFieldResultFilterAttribute(IProtectionProviderService protectionProviderService,
        ILogger<EncryptedFieldResultFilterAttribute> logger)
    {
        _protectionProviderService = protectionProviderService ??
                                     throw new ArgumentNullException(nameof(protectionProviderService));

        ProtectionProviderService = protectionProviderService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Logger = logger;
    }

    /// <summary>
    ///     An IProtectionProvider
    /// </summary>
    /// <value></value>
    public IProtectionProviderService ProtectionProviderService { get; }

    /// <summary>
    ///     The current ILogger
    /// </summary>
    /// <value></value>
    public ILogger<EncryptedFieldResultFilterAttribute> Logger { get; }

    /// <summary>
    ///     This filter encrypts the outgoing Models of action methods, before sending them to the client.
    /// </summary>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

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
                    encryptProperties(item);
                }
            }
        }
        else
        {
            encryptProperties(model);
        }
    }

    private void encryptProperties(object? model)
    {
        if (model == null)
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
            var attribute = property.GetCustomAttributes(typeof(EncryptedFieldAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                continue;
            }

            hasEncryptedFieldAttribute = true;

            var value = property.GetValue(model);

            if (value == null)
            {
                continue;
            }

            if (value is not string)
            {
                _logger.LogWarning(
                    "[EncryptedField] should be applied to `string` properties, But type of `{PropertyDeclaringType}.{PropertyName}` is `{PropertyPropertyType}`.",
                    property.DeclaringType, property.Name, property.PropertyType);

                continue;
            }

            var inputText = value.ToString();

            if (inputText == null)
            {
                continue;
            }

            var encryptedData = _protectionProviderService.Encrypt(inputText);
            property.SetValue(model, encryptedData);
        }

        _modelsWithEncryptedFieldAttributes.TryAdd(modelType, hasEncryptedFieldAttribute);
    }
}