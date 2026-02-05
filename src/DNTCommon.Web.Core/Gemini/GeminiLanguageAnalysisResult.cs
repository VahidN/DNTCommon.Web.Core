namespace DNTCommon.Web.Core;

public sealed class GeminiLanguageAnalysisResult
{
    public string Translation { get; init; } = string.Empty;

    public string Explanation { get; init; } = string.Empty;

    public string CloudComputing { get; init; } = string.Empty;

    public string MachineLearning { get; init; } = string.Empty;

    public string OpenSource { get; init; } = string.Empty;

    public string DataStructure { get; init; } = string.Empty;

    public int? ConfidenceRating { get; init; }
}
