#if NET_9 || NET_8
using Microsoft.AspNetCore.Components;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides a mechanism for rendering components non-interactively as HTML markup.
/// </summary>
public interface IBlazorStaticRendererService
{
    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    Task<string> StaticRenderComponentAsync<T>()
        where T : IComponent;

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    Task<string> StaticRenderComponentAsync(Type componentType);

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    Task<string> StaticRenderComponentAsync<T>(IDictionary<string, object?>? dictionary)
        where T : IComponent;

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    Task<string> StaticRenderComponentAsync(Type componentType, IDictionary<string, object?>? dictionary);
}
#endif
