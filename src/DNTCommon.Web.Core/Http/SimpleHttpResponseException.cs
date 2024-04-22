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
    public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        => StatusCode = statusCode;

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
    ///     Contains the values of status codes defined for HTTP.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
}