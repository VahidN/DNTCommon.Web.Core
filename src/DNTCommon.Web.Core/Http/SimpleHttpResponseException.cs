using System;
using System.Net;
using System.Runtime.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
/// Simple HttpResponse Exception
/// </summary>
[Serializable]
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

    /// <summary>
    /// Simple HttpResponse Exception
    /// </summary>
    protected SimpleHttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        var value = info.GetValue("StatusCode", typeof(HttpStatusCode));
        if (value != null)
        {
            StatusCode = (HttpStatusCode)value;
        }
    }

    /// <summary>
    /// Simple HttpResponse Exception
    /// </summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        base.GetObjectData(info, context);
        info.AddValue("StatusCode", StatusCode, typeof(HttpStatusCode));
    }
}