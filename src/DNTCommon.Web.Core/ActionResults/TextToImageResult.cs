using System;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
/// An ASP.NET Core text to image renderer.
/// </summary>
#if !NETCORE3_1
[SupportedOSPlatform("windows")]
#endif
public class TextToImageResult : ActionResult
{
    private readonly TextToImageOptions _options;
    private readonly string _text;

    /// <summary>
    /// An ASP.NET Core text to image renderer.
    /// </summary>
    public TextToImageResult(string text, TextToImageOptions options)
    {
        _options = options;
        _text = text;
    }

    /// <summary>
    /// Executes the result operation of the action method asynchronously.
    /// </summary>
#if !NETCORE3_1
    [SupportedOSPlatform("windows")]
#endif
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        return writeToResponseAsync(context);
    }

#if !NETCORE3_1
    [SupportedOSPlatform("windows")]
#endif
    private Task writeToResponseAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = new MediaTypeHeaderValue("image/png").ToString();
        context.HttpContext.DisableBrowserCache();
        var data = _text.TextToImage(_options);
        return response.Body.WriteAsync(data.AsMemory(0, data.Length)).AsTask();
    }
}