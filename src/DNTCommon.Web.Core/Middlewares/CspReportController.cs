using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// CSP Posted Model
    /// </summary>
    public class CspPost
    {
        /// <summary>
        /// The posted errors data
        /// </summary>
        [JsonPropertyName("csp-report")]
        public CspReport CspReport { get; set; }
    }

    /// <summary>
    /// The posted errors data
    /// </summary>
    public class CspReport
    {
        /// <summary>
        /// Such as "http://localhost:5000/untypedSha"
        /// </summary>
        [JsonPropertyName("document-uri")]
        public string DocumentUri { get; set; }

        /// <summary>
        /// The Referrer.
        /// </summary>
        [JsonPropertyName("referrer")]
        public string Referrer { get; set; }

        /// <summary>
        /// Such as "script-src"
        /// </summary>
        [JsonPropertyName("violated-directive")]
        public string ViolatedDirective { get; set; }

        /// <summary>
        /// Such as "script-src"
        /// </summary>
        [JsonPropertyName("effective-directive")]
        public string EffectiveDirective { get; set; }

        /// <summary>
        /// The Original Policy
        /// </summary>
        [JsonPropertyName("original-policy")]
        public string OriginalPolicy { get; set; }

        /// <summary>
        /// Such as "enforce"
        /// </summary>
        [JsonPropertyName("disposition")]
        public string Disposition { get; set; }

        /// <summary>
        /// Such as "eval"
        /// </summary>
        [JsonPropertyName("blocked-uri")]
        public string BlockedUri { get; set; }

        /// <summary>
        /// The LineNumber of the error
        /// </summary>
        [JsonPropertyName("line-number")]
        public int LineNumber { get; set; }

        /// <summary>
        /// The ColumnNumber of the error
        /// </summary>
        [JsonPropertyName("column-number")]
        public int ColumnNumber { get; set; }

        /// <summary>
        /// The SourceFile of the error
        /// </summary>
        [JsonPropertyName("source-file")]
        public string SourceFile { get; set; }

        /// <summary>
        /// Such as 200
        /// </summary>
        [JsonPropertyName("status-code")]
        public int StatusCode { get; set; }

        /// <summary>
        /// The Script Sample
        /// </summary>
        [JsonPropertyName("script-sample")]
        public string ScriptSample { get; set; }
    }

    /// <summary>
    /// Logs the ContentSecurityPolicy errors
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CspReportController : ControllerBase
    {
        private readonly ILogger<CspReportController> _logger;

        /// <summary>
        /// Logs the ContentSecurityPolicy errors
        /// </summary>
        public CspReportController(ILogger<CspReportController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Logs the ContentSecurityPolicy errors
        /// </summary>
        [HttpPost("[action]")]
        [EnableReadableBodyStream]
        public async Task<IActionResult> Log()
        {
            /* a sample payload:
            {
                "csp-report": {
                "document-uri": "http://localhost:5000/untypedSha",
                "referrer": "",
                "violated-directive": "script-src",
                "effective-directive": "script-src",
                "original-policy": "default-src 'self'; style-src 'self'; script-src 'self'; font-src 'self'; img-src 'self' data:; connect-src 'self'; media-src 'self'; object-src 'self'; report-uri /api/Home/CspReport",
                "disposition": "enforce",
                "blocked-uri": "eval",
                "line-number": 21,
                "column-number": 8,
                "source-file": "http://localhost:5000/scripts.bundle.js",
                "status-code": 200,
                "script-sample": ""
                }
            }
            */

            CspPost cspPost;
            using (var bodyReader = new StreamReader(this.HttpContext.Request.Body))
            {
                var body = await bodyReader.ReadToEndAsync();

                _logger.LogError($"Content Security Policy Error: {body}");

                this.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                cspPost = JsonSerializer.Deserialize<CspPost>(body);
            }

            return Ok();
        }
    }
}