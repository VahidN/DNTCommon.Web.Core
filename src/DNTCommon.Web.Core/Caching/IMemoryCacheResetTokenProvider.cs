using Microsoft.Extensions.Primitives;

namespace DNTCommon.Web.Core;

/// <summary>
///     Propagates notifications that a change has occurred.
/// </summary>
public interface IMemoryCacheResetTokenProvider
{
    /// <summary>
    ///     Gets or adds a change notification token.
    /// </summary>
    IChangeToken GetChangeToken(string key);

    /// <summary>
    ///     Removes a change notification token.
    /// </summary>
    void RemoveChangeToken(string key);

    /// <summary>
    ///     Gets all the defined tags. Each tag allows multiple cache entries to be considered as a group.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<string> GetTags();

    /// <summary>
    ///     Removes all the change notification tokens.
    /// </summary>
    void RemoveAllChangeTokens();
}