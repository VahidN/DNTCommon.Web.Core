using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     To use it, first you should add it to the ConstraintMap:
///     builder.Services.Configure&lt;RouteOptions&gt;(opt =&gt;
///     {
///     opt.ConstraintMap.Add(EncryptedRouteConstraint.Name, typeof(EncryptedRouteConstraint));
///     });
///     Then this new `encrypt` routing constraint can be used this way:
///     @page "/post/edit/{EditId:encrypt}"
/// </summary>
public class EncryptedRouteConstraint(
    IProtectionProviderService protectionProvider,
    ILogger<EncryptedRouteConstraint> logger) : IRouteConstraint
{
    /// <summary>
    ///     The name of the new routing constraint
    /// </summary>
    public const string Name = "encrypt";

    /// <inheritdoc />
    public bool Match(HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        ArgumentNullException.ThrowIfNull(routeKey);
        ArgumentNullException.ThrowIfNull(values);

        if (!values.TryGetValue(routeKey, out var routeValue))
        {
            return false;
        }

        try
        {
            var valueString = routeValue.ToInvariantString();
            values[routeKey] = protectionProvider.Decrypt(valueString);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Demystify(), message: "Invalid routing data.");

            return false;
        }
    }
}
