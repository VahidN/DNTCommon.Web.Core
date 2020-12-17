using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// HttpResponseMessage Extensions
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Includes the response's body in the final error message.
        /// </summary>
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = $"StatusCode: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
            response.Content?.Dispose();
            throw new SimpleHttpResponseException(response.StatusCode, content);
        }
    }
}