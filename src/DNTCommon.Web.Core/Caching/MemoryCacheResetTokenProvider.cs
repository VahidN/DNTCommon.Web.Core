using System.Collections.Concurrent;
using Microsoft.Extensions.Primitives;

namespace DNTCommon.Web.Core;

/// <summary>
///     Propagates notifications that a change has occurred.
/// </summary>
public class MemoryCacheResetTokenProvider : IMemoryCacheResetTokenProvider
{
    private readonly ConcurrentDictionary<string, ChangeTokenInfo> _changeTokens;

    /// <summary>
    ///     Propagates notifications that a change has occurred.
    /// </summary>
    public MemoryCacheResetTokenProvider()
        => _changeTokens = new ConcurrentDictionary<string, ChangeTokenInfo>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets or adds a change notification token.
    /// </summary>
    public IChangeToken GetChangeToken(string key)
        => _changeTokens.GetOrAdd(key, _ =>
            {
                using var cancellationTokenSource = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(cancellationTokenSource.Token);

                return new ChangeTokenInfo(changeToken, cancellationTokenSource);
            })
            .ChangeToken;

    /// <summary>
    ///     Removes a change notification token.
    /// </summary>
    public void RemoveChangeToken(string key)
    {
        if (_changeTokens.TryRemove(key, out var changeTokenInfo))
        {
            changeTokenInfo.TokenSource.Cancel();
        }
    }

    /// <summary>
    ///     Removes all the change notification tokens.
    /// </summary>
    public void RemoveAllChangeTokens()
    {
        foreach (var item in _changeTokens)
        {
            RemoveChangeToken(item.Key);
        }
    }

    /// <summary>
    ///     Gets all the defined tags. Each tag allows multiple cache entries to be considered as a group.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<string> GetTags() => _changeTokens.Select(x => x.Key).ToList();

    private readonly struct ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource)
    {
        public IChangeToken ChangeToken { get; } = changeToken;

        public CancellationTokenSource TokenSource { get; } = tokenSource;
    }
}
