using System;
using System.Net.Http;

namespace DNTCommon.Web.Core;

/// <summary>
/// A typed HttpClient
/// </summary>
public class BaseHttpClient
{
    /// <summary>
    /// A typed HttpClient
    /// </summary>
    public HttpClient HttpClient { get; }

    /// <summary>
    /// A typed HttpClient
    /// </summary>
    /// <param name="httpClient"></param>
    public BaseHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}