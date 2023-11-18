using System.Runtime.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Simple HttpResponse Exception
/// </summary>
[Serializable]
public class SimpleHttpResponseException : Exception
{
    /// <summary>
    ///     Simple HttpResponse Exception
    /// </summary>
    public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content) =>
        StatusCode = statusCode;

    /// <summary>
    ///     Simple HttpResponse Exception
    /// </summary>
    public SimpleHttpResponseException()
    {
    }

    /// <summary>
    ///     Simple HttpResponse Exception
    /// </summary>
    public SimpleHttpResponseException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Simple HttpResponse Exception
    /// </summary>
    public SimpleHttpResponseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Simple HttpResponse Exception
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
    ///     Contains the values of status codes defined for HTTP.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     Simple HttpResponse Exception
    /// </summary>
#if NET_8
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.",
                 DiagnosticId = "SYSLIB0051",
                 UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
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