using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
/// CSP Posted Model
/// </summary>
public class CspPost
{
    /// <summary>
    /// The posted errors data
    /// </summary>
    [JsonPropertyName("csp-report")]
    public CspReport CspReport { get; set; } = new CspReport();
}