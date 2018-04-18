using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Http Request Info Extensions
    /// </summary>
    public static class HttpRequestInfoServiceExtensions
    {
        /// <summary>
        /// Adds IHttpContextAccessor, IActionContextAccessor, IUrlHelper and IHttpRequestInfoService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddHttpRequestInfoService(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Allows injecting IUrlHelper as a dependency
            services.AddScoped(serviceProvider =>
            {
                var actionContext = serviceProvider.GetService<IActionContextAccessor>().ActionContext;
                var urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();
                return urlHelperFactory?.GetUrlHelper(actionContext);
            });
            services.AddScoped<IHttpRequestInfoService, HttpRequestInfoService>();
            return services;
        }
    }

    /// <summary>
    /// HttpRequest Info
    /// </summary>
    public interface IHttpRequestInfoService
    {
        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        string GetIP(bool tryUseXForwardHeader = true);

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        string GetHeaderValue(string headerName);

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        string GetUserAgent();

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        Uri AbsoluteContent(string contentPath);

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        Uri GetBaseUri();

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        string GetBaseUrl();

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        string GetRawUrl();

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        Uri GetRawUri();

        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        IUrlHelper GetUrlHelper();

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        Task<T> DeserializeRequestJsonBodyAsAsync<T>();

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        Task<string> ReadRequestBodyAsStringAsync();

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        Task<Dictionary<string, string>> DeserializeRequestJsonBodyAsDictionaryAsync();
    }

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
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        public string GetUserAgent()
        {
            return GetHeaderValue("User-Agent");
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        public string GetIP(bool tryUseXForwardHeader = true)
        {
            string ip = string.Empty;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
            {
                ip = SplitCsv(GetHeaderValue("X-Forwarded-For")).FirstOrDefault();
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) &&
                _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = GetHeaderValue("REMOTE_ADDR");
            }

            return ip;
        }

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        public string GetHeaderValue(string headerName)
        {
            if (_httpContextAccessor.HttpContext.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                return values.ToString();
            }
            return string.Empty;
        }

        private static List<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
            {
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();
            }

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        public Uri AbsoluteContent(string contentPath)
        {
            var urlHelper = _urlHelper ?? throw new NullReferenceException(nameof(_urlHelper));
            return new Uri(GetBaseUri(), urlHelper.Content(contentPath));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public Uri GetBaseUri()
        {
            return new Uri(GetBaseUrl());
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public string GetBaseUrl()
        {
            requestSanityCheck();
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host.ToUriComponent()}";
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public string GetRawUrl()
        {
            requestSanityCheck();
            return _httpContextAccessor.HttpContext.Request.GetDisplayUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public Uri GetRawUri()
        {
            return new Uri(GetRawUrl());
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        public IUrlHelper GetUrlHelper()
        {
            return _urlHelper ?? throw new NullReferenceException(nameof(_urlHelper));
        }

        private void requestSanityCheck()
        {
            if (_httpContextAccessor == null)
            {
                throw new NullReferenceException("httpContextAccessor is null.");
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                throw new NullReferenceException("HttpContext is null.");
            }

            if (_httpContextAccessor.HttpContext.Request == null)
            {
                throw new NullReferenceException("HttpContext.Request is null.");
            }
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public async Task<T> DeserializeRequestJsonBodyAsAsync<T>()
        {
            requestSanityCheck();
            var request = _httpContextAccessor.HttpContext.Request;
            using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var body = await bodyReader.ReadToEndAsync();
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        public async Task<string> ReadRequestBodyAsStringAsync()
        {
            requestSanityCheck();
            var request = _httpContextAccessor.HttpContext.Request;
            using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var body = await bodyReader.ReadToEndAsync();
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                return body;
            }
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public async Task<Dictionary<string, string>> DeserializeRequestJsonBodyAsDictionaryAsync()
        {
            requestSanityCheck();
            var request = _httpContextAccessor.HttpContext.Request;
            using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var body = await bodyReader.ReadToEndAsync();
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            }
        }
    }
}