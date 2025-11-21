#if NET_10 || NET_9 || NET_8

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DNTCommon.Web.Core;

/// <summary>
///     Call it this way:
///     string html = await renderer.StaticRenderComponentAsync&lt;App&gt;();
/// </summary>
/// <remarks>
///     Provides a mechanism for rendering components non-interactively as HTML markup.
/// </remarks>
public class BlazorStaticRendererService(HtmlRenderer htmlRenderer) : IBlazorStaticRendererService
{
    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync<T>()
        where T : IComponent
        => RenderComponentAsync<T>(ParameterView.Empty);

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync<T>(IDictionary<string, object?>? dictionary)
        where T : IComponent
        => RenderComponentAsync<T>(dictionary is null ? ParameterView.Empty : ParameterView.FromDictionary(dictionary));

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync(Type componentType)
        => RenderComponentAsync(componentType, ParameterView.Empty);

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync(Type componentType, IDictionary<string, object?>? dictionary)
        => RenderComponentAsync(componentType,
            dictionary is null ? ParameterView.Empty : ParameterView.FromDictionary(dictionary));

    private Task<string> RenderComponentAsync<T>(ParameterView? parameters)
        where T : IComponent
        => htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<T>(parameters ?? ParameterView.Empty);

            return output.ToHtmlString();
        });

    private Task<string> RenderComponentAsync(Type componentType, ParameterView? parameters)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException(
                $"Invalid component type, `{componentType}`. It's not assignable from `IComponent`.",
                nameof(componentType));
        }

        return htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(componentType, parameters ?? ParameterView.Empty);

            return output.ToHtmlString();
        });
    }
}

#endif
