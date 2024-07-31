#if !NET_6
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     By design and documented, when a user is authenticated output caching is disabled for the request.
///     This policy doesn't have this restriction, and you can use it this way:
///     .AddOutputCache(options => { options.AddPolicy(AlwaysCachePolicy.Name, AlwaysCachePolicy.Instance); })
///     @attribute [OutputCache(Duration = TimeConstants.Minute * 5, PolicyName = AlwaysCachePolicy.Name)]
/// </summary>
public sealed class AlwaysCachePolicy : IOutputCachePolicy
{

    /// <summary>
    ///     The name of the Policy which can be used as
    ///     .AddOutputCache(options => { options.AddPolicy(AlwaysCachePolicy.Name, AlwaysCachePolicy.Instance); })
    /// </summary>
    public const string Name = nameof(AlwaysCachePolicy);

    /// <summary>
    ///     The Instance of the Policy which can be used as
    ///     .AddOutputCache(options => { options.AddPolicy(AlwaysCachePolicy.Name, AlwaysCachePolicy.Instance); })
    /// </summary>
    public static readonly AlwaysCachePolicy Instance = new();

    private AlwaysCachePolicy()
    {
    }

    ValueTask IOutputCachePolicy.CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var attemptOutputCaching = EnableOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by any query by default
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        => ValueTask.CompletedTask;

    ValueTask IOutputCachePolicy.ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var response = context.HttpContext.Response;

        // Verify existence of cookie headers
        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;

            return ValueTask.CompletedTask;
        }

        // Check response code
        if (response.StatusCode != StatusCodes.Status200OK &&
            response.StatusCode != StatusCodes.Status301MovedPermanently)
        {
            context.AllowCacheStorage = false;

            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    private static bool EnableOutputCaching(OutputCacheContext context)
    {
        var request = context.HttpContext.Request;

        return HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method) ||
               HttpMethods.IsPost(request.Method);
    }
}
#endif