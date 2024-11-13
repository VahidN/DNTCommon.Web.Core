using System.Collections.Concurrent;

namespace DNTCommon.Web.Core;

/// <summary>
///     Lifetime of this class should be set to `Singleton`.
/// </summary>
public class CommonHttpClientFactory : ICommonHttpClientFactory
{
    private const int ConnectionLeaseTimeout = 60 * 1000; // 1 minute

    // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the HttpClient more than
    // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
    // threads but only one of the objects succeeds in creating the HttpClient.
    private readonly ConcurrentDictionary<Uri, Lazy<HttpClient>> _httpClients = new();

    private bool _isDisposed;

    /// <summary>
    ///     Reusing a single HttpClient instance across a multi-threaded application
    /// </summary>
    public CommonHttpClientFactory()
    {
        // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
        ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromMinutes(value: 1).TotalMilliseconds;

        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = 1024;
    }

    /// <summary>
    ///     Reusing a single HttpClient instance across a multi-threaded application
    /// </summary>
    public HttpClient GetOrCreate(Uri baseAddress,
        IDictionary<string, string>? defaultRequestHeaders = null,
        TimeSpan? timeout = null,
        long? maxResponseContentBufferSize = null,
        HttpMessageHandler? handler = null)
        => _httpClients.GetOrAdd(baseAddress, uri => new Lazy<HttpClient>(() =>
            {
                // Reusing a single HttpClient instance across a multi-threaded application means
                // you can't change the values of the stateful properties (which are not thread safe),
                // like BaseAddress, DefaultRequestHeaders, MaxResponseContentBufferSize and Timeout.
                // So you can only use them if they are constant across your application and need their own instance if being varied.
                var client = handler == null
                    ? new HttpClient
                    {
                        BaseAddress = uri
                    }
                    : new HttpClient(handler, disposeHandler: false)
                    {
                        BaseAddress = uri
                    };

                SetRequestTimeout(timeout, client);
                SetMaxResponseBufferSize(maxResponseContentBufferSize, client);
                SetDefaultHeaders(defaultRequestHeaders, client);
                SetConnectionLeaseTimeout(uri, client);

                return client;
            }, LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;

    /// <summary>
    ///     Dispose all of the httpClients
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Dispose all of the httpClients
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            try
            {
                if (disposing)
                {
                    foreach (var httpClient in _httpClients.Values)
                    {
                        httpClient.Value.Dispose();
                    }
                }
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }

    private static void SetConnectionLeaseTimeout(Uri baseAddress, HttpClient client)
    {
        // This ensures connections are used efficiently but not indefinitely.
        client.DefaultRequestHeaders.ConnectionClose =
            false; // keeps the connection open -> more efficient use of the client

        ServicePointManager.FindServicePoint(baseAddress).ConnectionLeaseTimeout =
            ConnectionLeaseTimeout; // ensures connections are not used indefinitely.
    }

    private static void SetDefaultHeaders(IDictionary<string, string>? defaultRequestHeaders, HttpClient client)
    {
        if (defaultRequestHeaders == null)
        {
            return;
        }

        foreach (var item in defaultRequestHeaders)
        {
            client.DefaultRequestHeaders.Add(item.Key, item.Value);
        }
    }

    private static void SetMaxResponseBufferSize(long? maxResponseContentBufferSize, HttpClient client)
    {
        if (maxResponseContentBufferSize.HasValue)
        {
            client.MaxResponseContentBufferSize = maxResponseContentBufferSize.Value;
        }
    }

    private static void SetRequestTimeout(TimeSpan? timeout, HttpClient client)
    {
        if (timeout.HasValue)
        {
            client.Timeout = timeout.Value;
        }
    }
}