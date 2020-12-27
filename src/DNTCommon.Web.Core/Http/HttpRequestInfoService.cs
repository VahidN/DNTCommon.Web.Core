using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Http Request Info
    /// </summary>
    public class HttpRequestInfoService : IHttpRequestInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Http Request Info
        /// </summary>
        public HttpRequestInfoService(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        public string? GetUserAgent()
        {
            return GetHeaderValue("User-Agent");
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public string? GetReferrerUrl()
        {
            return _httpContextAccessor.HttpContext?.GetReferrerUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public Uri? GetReferrerUri()
        {
            return _httpContextAccessor.HttpContext?.GetReferrerUri();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        public string? GetIP(bool tryUseXForwardHeader = true)
        {
            return _httpContextAccessor.HttpContext?.GetIP(tryUseXForwardHeader);
        }

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        public string? GetHeaderValue(string headerName)
        {
            return _httpContextAccessor.HttpContext?.GetHeaderValue(headerName);
        }

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        public Uri? AbsoluteContent(string contentPath)
        {
            var baseUri = GetBaseUri();
            return baseUri == null ? null : new Uri(baseUri, _urlHelper.Content(contentPath));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public Uri? GetBaseUri()
        {
            var uriString = GetBaseUrl();
            return uriString == null ? null : new Uri(uriString);
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public string? GetBaseUrl()
        {
            return _httpContextAccessor.HttpContext?.GetBaseUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public string? GetRawUrl()
        {
            return _httpContextAccessor.HttpContext?.GetRawUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public Uri? GetRawUri()
        {
            var uriString = GetRawUrl();
            return uriString == null ? null : new Uri(uriString);
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        public IUrlHelper GetUrlHelper()
        {
            return _urlHelper;
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public Task<T?> DeserializeRequestJsonBodyAsAsync<T>()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Task.FromResult(default(T));
            }

            return _httpContextAccessor.HttpContext.DeserializeRequestJsonBodyAsAsync<T>();
        }

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        public Task<string> ReadRequestBodyAsStringAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Task.FromResult(string.Empty);
            }

            return _httpContextAccessor.HttpContext.ReadRequestBodyAsStringAsync();
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public Task<IDictionary<string, string>?> DeserializeRequestJsonBodyAsDictionaryAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Task.FromResult<IDictionary<string, string>?>(null);
            }

            return _httpContextAccessor.HttpContext.DeserializeRequestJsonBodyAsDictionaryAsync();
        }
    }
}