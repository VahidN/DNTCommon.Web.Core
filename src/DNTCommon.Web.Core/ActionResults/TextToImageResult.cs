using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core text to image renderer.
/// </summary>
public class TextToImageResult(string text, TextToImageOptions options) : ActionResult
{
    /// <summary>
    ///     Executes the result operation of the action method asynchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return WriteToResponseAsync(context);
    }

    private Task WriteToResponseAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = new MediaTypeHeaderValue(mediaType: "image/png").ToString();
        context.HttpContext.DisableBrowserCache();
        var data = text.TextToImage(options);

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }
}
