#if NETSTANDARD2_0 || NETCOREAPP3_0 || NETCOREAPP3_1
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
#if NETSTANDARD2_0
using System.Net.Http.Formatting;
#endif

#if NETCOREAPP3_0 || NETCOREAPP3_1
using System.Text.Json;
using System.Text;
#endif

namespace DNTCommon.Web.Core.Http
{
    /// <summary>
    /// HttpClient Extensions
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Allows manipulation of the request headers before it is sent, when you are using a signelton httpClient.
        /// </summary>
        public static Task<HttpResponseMessage> GetAsync(
            this HttpClient httpClient,
            string uri,
            Action<HttpRequestMessage> preAction)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            preAction(httpRequestMessage);
            return httpClient.SendAsync(httpRequestMessage);
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Allows manipulation of the request headers before it is sent, when you are using a signelton httpClient.
        /// </summary>
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient,
            string uri,
            T value,
            Action<HttpRequestMessage> preAction)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new ObjectContent<T>(value, new JsonMediaTypeFormatter(), (MediaTypeHeaderValue)null)
            };
            preAction(httpRequestMessage);
            return httpClient.SendAsync(httpRequestMessage);
        }
#endif

#if NETCOREAPP3_0 || NETCOREAPP3_1
        /// <summary>
        /// Allows manipulation of the request headers before it is sent, when you are using a signelton httpClient.
        /// </summary>
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient,
            string uri,
            T value,
            Action<HttpRequestMessage> preAction)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
            };
            preAction(httpRequestMessage);
            return httpClient.SendAsync(httpRequestMessage);
        }
#endif
    }
}
#endif