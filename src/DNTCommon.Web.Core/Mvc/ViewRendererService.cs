using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace DNTCommon.Web.Core;

/// <summary>
///     Modified version of:
///     https://github.com/aspnet/Entropy/blob/dev/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
/// </summary>
/// <remarks>
///     Renders a .cshtml file as a string.
/// </remarks>
public class ViewRendererService(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor) : IViewRendererService
{
    /// <summary>
    ///     Renders a .cshtml file as a string.
    /// </summary>
    public Task<string> RenderViewToStringAsync(string viewNameOrPath)
        => RenderViewToStringAsync(viewNameOrPath, string.Empty);

#pragma warning disable CC001,MA0137
    /// <summary>
    ///     Renders a .cshtml file as a string.
    /// </summary>
    public async Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model)
#pragma warning restore CC001, MA0137
    {
        var actionContext = GetActionContext();

        var viewEngineResult = viewEngine.FindView(actionContext, viewNameOrPath, isMainPage: false);

        if (!viewEngineResult.Success)
        {
            viewEngineResult = viewEngine.GetView(executingFilePath: "~/", viewNameOrPath, isMainPage: false);

            if (!viewEngineResult.Success)
            {
                throw new FileNotFoundException($"Couldn't find '{viewNameOrPath}'");
            }
        }

        var view = viewEngineResult.View;

        await using var output = new StringWriter();

        var viewDataDictionary =
            new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

        var viewContext = new ViewContext(actionContext, view, viewDataDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider), output, new HtmlHelperOptions());

        await view.RenderAsync(viewContext);

        return output.ToString();
    }

    private ActionContext GetActionContext()
    {
        var httpContext = httpContextAccessor?.HttpContext;

        if (httpContext is not null)
        {
            return new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
        }

        httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }
}
