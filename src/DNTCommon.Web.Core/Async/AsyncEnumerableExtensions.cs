using System.Collections.Generic;

namespace DNTCommon.Web.Core;

/// <summary>
/// C# 9.0 GetEnumerator Extensions
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// C# 9.0 GetEnumerator Extensions
    /// </summary>
    public static IEnumerator<T> GetEnumerator<T>(this IEnumerator<T> enumerator) => enumerator;

    /// <summary>
    /// C# 9.0 GetEnumerator Extensions
    /// </summary>
    public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IAsyncEnumerator<T> enumerator) => enumerator;
}