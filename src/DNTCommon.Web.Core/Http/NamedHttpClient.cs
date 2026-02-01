namespace DNTCommon.Web.Core;

public static class NamedHttpClient
{
    public const string BaseHttpClient = nameof(BaseHttpClient);

    public const string BaseHttpClientWithoutAutoRedirect = nameof(BaseHttpClientWithoutAutoRedirect);

    extension(IHttpClientFactory httpClientFactory)
    {
        public HttpClient CreateBaseHttpClient()
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);

            return httpClientFactory.CreateClient(BaseHttpClient);
        }

        public HttpClient CreateBaseHttpClientWithoutAutoRedirect()
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);

            return httpClientFactory.CreateClient(BaseHttpClientWithoutAutoRedirect);
        }
    }
}
