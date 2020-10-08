using System;
using System.Net;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Simple HttpResponse Exception
    /// </summary>
    public class SimpleHttpResponseException : Exception
    {
        /// <summary>
        /// Contains the values of status codes defined for HTTP.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Simple HttpResponse Exception
        /// </summary>
        public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}