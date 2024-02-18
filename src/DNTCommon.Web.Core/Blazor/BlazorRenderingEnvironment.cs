namespace DNTCommon.Web.Core;

/// <summary>
///     Determines the current rendering environment of a Blazor page.
/// </summary>
public enum BlazorRenderingEnvironment
{
    /// <summary>
    ///     This is an unknown environment!
    /// </summary>
    Unknown,

    /// <summary>
    ///     Running from a console or background-service project.
    /// </summary>
    Console,

    /// <summary>
    ///     Rendering from the Client project. Using HTTP request for connectivity.
    /// </summary>
    Wasm,

    /// <summary>
    ///     Rendering from the Server project. Indicates if the response has started rendering.
    /// </summary>
    SsrPrerendering,

    /// <summary>
    ///     Rendering from the Server project. Using WebSockets for connectivity.
    /// </summary>
    InteractiveServer,

    /// <summary>
    ///     Rendering from a WebView.
    /// </summary>
    WebView
}