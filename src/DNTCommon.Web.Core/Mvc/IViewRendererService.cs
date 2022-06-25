using System.Threading.Tasks;

namespace DNTCommon.Web.Core;

/// <summary>
/// More info: http://www.dotnettips.info/post/2564
/// </summary>
public interface IViewRendererService
{
    /// <summary>
    /// Renders a .cshtml file as an string.
    /// </summary>
    Task<string> RenderViewToStringAsync(string viewNameOrPath);

    /// <summary>
    /// Renders a .cshtml file as an string.
    /// </summary>
    Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model);
}