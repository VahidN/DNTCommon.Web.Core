using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     MemoryCacheEntryOptions Extensions
/// </summary>
public static class MemoryCacheEntryOptionsExtensions
{
    /// <summary>
    ///     It's 30 seconds
    /// </summary>
    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromSeconds(value: 30);

    /// <summary>
    ///     Creates a new MemoryCacheEntryOptions based on the provided options
    /// </summary>
    /// <param name="expiresOn">Gets or sets the exact <see cref="DateTimeOffset" /> the cache entry should be evicted.</param>
    /// <param name="expiresAfter">
    ///     Gets or sets the duration, from the time the cache entry was added, when it should be
    ///     evicted.
    /// </param>
    /// <param name="expiresSliding">Gets or sets the duration from last access that the cache entry should be evicted.</param>
    /// <param name="priority">Gets or sets the <see cref="CacheItemPriority" /> policy for the cache entry.</param>
    /// <returns></returns>
    public static MemoryCacheEntryOptions GetMemoryCacheEntryOptions(this DateTimeOffset? expiresOn,
        TimeSpan? expiresAfter = null,
        TimeSpan? expiresSliding = null,
        CacheItemPriority? priority = null)
    {
        var hasEvictionCriteria = false;
        var options = new MemoryCacheEntryOptions();
        options.SetSize(size: 1);

        if (expiresOn is not null)
        {
            hasEvictionCriteria = true;
            options.SetAbsoluteExpiration(expiresOn.Value);
        }

        if (expiresAfter is not null)
        {
            hasEvictionCriteria = true;
            options.SetAbsoluteExpiration(expiresAfter.Value);
        }

        if (expiresSliding is not null)
        {
            hasEvictionCriteria = true;
            options.SetSlidingExpiration(expiresSliding.Value);
        }

        if (priority is not null)
        {
            options.SetPriority(priority.Value);
        }

        if (!hasEvictionCriteria)
        {
            options.SetSlidingExpiration(DefaultExpiration);
        }

        return options;
    }
}
