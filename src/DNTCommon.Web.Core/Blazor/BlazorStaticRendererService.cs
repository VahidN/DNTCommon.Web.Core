#if NET_8

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DNTCommon.Web.Core;

/// <summary>
///     Call it this way:
///     string html = await renderer.StaticRenderComponentAsync&lt;App&gt;();
/// </summary>
public class BlazorStaticRendererService : IBlazorStaticRendererService
{
    private readonly HtmlRenderer _htmlRenderer;

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public BlazorStaticRendererService(HtmlRenderer htmlRenderer) => _htmlRenderer = htmlRenderer;

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync<T>() where T : IComponent
        => RenderComponentAsync<T>(ParameterView.Empty);

    /// <summary>
    ///     Provides a mechanism for rendering components non-interactively as HTML markup.
    /// </summary>
    public Task<string> StaticRenderComponentAsync<T>(IDictionary<string, object?> dictionary) where T : IComponent
        => RenderComponentAsync<T>(ParameterView.FromDictionary(dictionary));

    private Task<string> RenderComponentAsync<T>(ParameterView parameters) where T : IComponent =>
        _htmlRenderer.Dispatcher.InvokeAsync(async () =>
                                             {
                                                 var output = await _htmlRenderer.RenderComponentAsync<T>(parameters);
                                                 return output.ToHtmlString();
                                             });
}

#endif