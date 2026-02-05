namespace DNTCommon.Web.Core;

public interface IGeminiLanguageAnalysisService
{
    Task<GeminiLanguageAnalysisResult?> GetGeminiPersianLanguageAnalysisResultAsync(string apiKey,
        string model,
        CancellationToken cancellationToken = default);

    GeminiLanguageAnalysisResult? GetGeminiPersianLanguageAnalysisResult(string? rawAiOutput);

    Task<IList<GeminiModelInfo>?> GetGeminiModelsWithPersianLanguageSupportAsync(string apiKey,
        int minConfidenceRating = 8,
        CancellationToken cancellationToken = default);
}
