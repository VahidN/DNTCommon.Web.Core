using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
/// The posted errors data
/// </summary>
public class CspReport
{
    /// <summary>
    /// Such as "http://localhost:5000/untypedSha"
    /// </summary>
    [JsonPropertyName("document-uri")]
    public string? DocumentUri { get; set; }

    /// <summary>
    /// The Referrer.
    /// </summary>
    [JsonPropertyName("referrer")]
    public string? Referrer { get; set; }

    /// <summary>
    /// Such as "script-src"
    /// </summary>
    [JsonPropertyName("violated-directive")]
    public string? ViolatedDirective { get; set; }

    /// <summary>
    /// Such as "script-src"
    /// </summary>
    [JsonPropertyName("effective-directive")]
    public string? EffectiveDirective { get; set; }

    /// <summary>
    /// The Original Policy
    /// </summary>
    [JsonPropertyName("original-policy")]
    public string? OriginalPolicy { get; set; }

    /// <summary>
    /// Such as "enforce"
    /// </summary>
    [JsonPropertyName("disposition")]
    public string? Disposition { get; set; }

    /// <summary>
    /// Such as "eval"
    /// </summary>
    [JsonPropertyName("blocked-uri")]
    public string? BlockedUri { get; set; }

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
    public string? SourceFile { get; set; }

    /// <summary>
    /// Such as 200
    /// </summary>
    [JsonPropertyName("status-code")]
    public int StatusCode { get; set; }

    /// <summary>
    /// The Script Sample
    /// </summary>
    [JsonPropertyName("script-sample")]
    public string? ScriptSample { get; set; }
}