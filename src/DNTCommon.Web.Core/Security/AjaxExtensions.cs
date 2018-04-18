using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2518
    /// </summary>
    public static class AjaxExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        /// <summary>
        /// Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
        /// </summary>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request?.Headers != null && request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }
    }

    /// <summary>
    /// Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
    /// </summary>
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// Determines whether the action selection is valid for the specified route context.
        /// </summary>
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}