using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///   Represents a result that renders text as an image using specified options.
///   This result sets the content type to "image/png", disables browser caching,
///   converts the provided text to an image, and writes the image data to the response body.
/// </summary>
/// <param name="text">The text to be rendered as an image.</param>
/// <param name="options">The options to use when rendering the text as an image.</param>
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
