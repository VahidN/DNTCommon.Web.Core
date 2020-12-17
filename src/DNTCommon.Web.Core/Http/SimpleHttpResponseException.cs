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

        /// <summary>
        /// Simple HttpResponse Exception
        /// </summary>
        public SimpleHttpResponseException() : base()
        {
        }

        /// <summary>
        /// Simple HttpResponse Exception
        /// </summary>
        public SimpleHttpResponseException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Simple HttpResponse Exception
        /// </summary>
        public SimpleHttpResponseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}