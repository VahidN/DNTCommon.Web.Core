using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     More info: http://www.dntips.ir/post/2518
/// </summary>
public static class AjaxExtensions
{
    private const string RequestedWithHeader = "X-Requested-With";
    private const string XmlHttpRequest = "XMLHttpRequest";

    /// <summary>
    ///     Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
    /// </summary>
    public static bool IsAjaxRequest(this HttpRequest request)
        => request?.Headers is not null && request.Headers[RequestedWithHeader] == XmlHttpRequest;
}
