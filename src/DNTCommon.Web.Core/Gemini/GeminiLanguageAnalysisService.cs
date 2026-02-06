using System.Text.RegularExpressions;
using DNTPersianUtils.Core;

namespace DNTCommon.Web.Core;

public class GeminiLanguageAnalysisService(IGeminiClientService geminiClientService) : IGeminiLanguageAnalysisService
{
    public const string PersianLanguageAnalysisPrompt = """
                                                        You are a language testing agent. Your task is to objectively evaluate your ability to handle complex Persian (Farsi) tasks related to programming news.

                                                        You MUST perform the following three tasks in Persian and then rate your confidence.

                                                        --- Task Set ---
                                                        1. **Translation & Summarization:** Translate the following English sentence into a concise, professional Persian sentence and then summarize the core technical context in one additional Persian sentence.
                                                           Input: "The new React Router V6.5 introduced nested routing and deferred loading capabilities using Suspense."

                                                        2. **Technical Explanation:** Explain the concept of 'Containerization using Docker' in two to three clear and factual Persian sentences.

                                                        3. **Persian Terminology Check:** Provide the Persian equivalent for the following four English terms (A-D) in the required format.
                                                           A. Cloud Computing
                                                           B. Machine Learning
                                                           C. Open Source
                                                           D. Data Structure

                                                        --- Output Format ---
                                                        Each section must start with its label on a new line.
                                                        Do not merge labels and content on the same line.
                                                        Your entire output MUST be a single block of text following this format exactly:

                                                        [START LANGUAGE ANALYSIS]
                                                        1. Translation Result: [Persian sentence 1. Persian sentence 2.]
                                                        2. Explanation Result: [Two to three Persian sentences explaining Docker.]
                                                        3. Terminology Results:
                                                           A. [Persian equivalent for Cloud Computing]
                                                           B. [Persian equivalent for Machine Learning]
                                                           C. [Persian equivalent for Open Source]
                                                           D. [Persian equivalent for Data Structure]
                                                        CONFIDENCE RATING (1-10): [An integer score from 1 (Very Low Confidence) to 10 (Very High Confidence) regarding the quality of your Persian output for the three tasks.]
                                                        [END LANGUAGE ANALYSIS]
                                                        """;

    public const string ConfidenceLabel = "CONFIDENCE RATING";

    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(seconds: 3);

    public static readonly string[] SectionLabels = ["1. Translation Result", "2. Explanation Result"];

    public static readonly string[] TerminologyLabels = ["A.", "B.", "C.", "D."];

    public async Task<GeminiLanguageAnalysisResult?> GetGeminiPersianLanguageAnalysisResultAsync(string apiKey,
        string model,
        CancellationToken cancellationToken = default)
    {
        var responseResult = await geminiClientService.RunGenerateContentPromptsAsync(new GeminiClientOptions
        {
            ApiVersion = GeminiApiVersions.V1Beta,
            ApiKey = apiKey,
            ModelId = model,
            Chats = [new GeminiChatRequest(PersianLanguageAnalysisPrompt)]
        }, cancellationToken);

        if (responseResult.IsSuccessfulResponse is null or false)
        {
            return null;
        }

        var rawAiOutput = responseResult.Result?.ResponseParts?.FirstOrDefault()?.Text;

        return GetGeminiPersianLanguageAnalysisResult(rawAiOutput);
    }

    public GeminiLanguageAnalysisResult? GetGeminiPersianLanguageAnalysisResult(string? rawAiOutput)
    {
        if (rawAiOutput.IsEmpty())
        {
            return null;
        }

        var repaired = Normalize(rawAiOutput);

        return Parse(repaired);
    }

    public async Task<IList<GeminiModelInfo>?> GetGeminiModelsWithPersianLanguageSupportAsync(string apiKey,
        int minConfidenceRating = 8,
        CancellationToken cancellationToken = default)
    {
        var models = await geminiClientService.GetGeminiModelsListAsync(apiKey, cancellationToken);

        if (models is null)
        {
            return null;
        }

        List<GeminiModelInfo> results = [];

        foreach (var model in models)
        {
            await Task.Delay(MatchTimeout, cancellationToken);

            var modelName = model.Name.TrimStart(value: "models/", StringComparison.OrdinalIgnoreCase);

            if (modelName.IsEmpty())
            {
                continue;
            }

            var result = await GetGeminiPersianLanguageAnalysisResultAsync(apiKey, modelName, cancellationToken);

            if (result is null)
            {
                continue;
            }

            if (ContainsFarsi(minConfidenceRating, result))
            {
                results.Add(model);
            }
        }

        return results;
    }

    private static bool ContainsFarsi(int minConfidenceRating, GeminiLanguageAnalysisResult result)
        => result.ConfidenceRating >= minConfidenceRating && result.Translation.ContainsFarsi() &&
           result.Explanation.ContainsFarsi() && result.CloudComputing.ContainsFarsi() &&
           result.MachineLearning.ContainsFarsi() && result.OpenSource.ContainsFarsi() &&
           result.DataStructure.ContainsFarsi();

    private static string BuildBoundaryRegex()
    {
        var allLabels = SectionLabels.Concat(TerminologyLabels).Append(ConfidenceLabel).Select(Regex.Escape);

        return $@"(?=\n(?:{string.Join(separator: '|', allLabels)})|\z)";
    }

    private static string ExtractSection(string input, string label)
    {
        var boundary = BuildBoundaryRegex();

        var pattern = $"""
                       {Regex.Escape(label)}\s*:\s*
                       (?<value>.*?)
                       {boundary}
                       """;

        var match = Regex.Match(input, pattern,
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace, MatchTimeout);

        return match.Success ? match.Groups[groupname: "value"].Value.Trim() : string.Empty;
    }

    private static string ExtractTerminology(string input, char key)
    {
        var boundary = BuildBoundaryRegex();

        var pattern = $"""
                       {key}\.\s*
                       (?<value>.*?)
                       {boundary}
                       """;

        var match = Regex.Match(input, pattern,
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace, MatchTimeout);

        return match.Success ? match.Groups[groupname: "value"].Value.Trim() : string.Empty;
    }

    private static GeminiLanguageAnalysisResult? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        return new GeminiLanguageAnalysisResult
        {
            Translation = ExtractSection(input, label: "1. Translation Result"),
            Explanation = ExtractSection(input, label: "2. Explanation Result"),
            CloudComputing = ExtractTerminology(input, key: 'A'),
            MachineLearning = ExtractTerminology(input, key: 'B'),
            OpenSource = ExtractTerminology(input, key: 'C'),
            DataStructure = ExtractTerminology(input, key: 'D'),
            ConfidenceRating = TryExtractConfidence(input)
        };
    }

    private static int? TryExtractConfidence(string input)
    {
        var match = Regex.Match(input, pattern: @"CONFIDENCE RATING\s*\(1-10\)\s*:\s*(?<value>\d+)",
            RegexOptions.IgnoreCase, MatchTimeout);

        return match.Success && int.TryParse(match.Groups[groupname: "value"].Value, out var v) ? v : null;
    }

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var text = input.Trim();

        //  یکدست‌سازی Line Ending
        text = text.Replace(oldValue: "\r\n", newValue: "\n", StringComparison.Ordinal);

        //  جدا کردن label و content اگر توی یک خط هستند
        text = Regex.Replace(text, pattern: @"(Translation Result|Explanation Result)\s*:\s*(.+)",
            replacement: "$1:\n$2", RegexOptions.IgnoreCase, MatchTimeout);

        //  اضافه کردن colon اگر حذف شده
        text = Regex.Replace(text, pattern: @"^(1\.\s*Translation Result|2\.\s*Explanation Result)\s*$",
            replacement: "$1:", RegexOptions.Multiline | RegexOptions.IgnoreCase, MatchTimeout);

        //  نرمال‌سازی Terminology بدون A/B/C/D
        text = NormalizeTerminology(text);

        //  نرمال‌سازی Confidence
        text = Regex.Replace(text, pattern: @"Confidence\s*(Rating)?\s*[=:]\s*(\d+)",
            replacement: "CONFIDENCE RATING (1-10): $2", RegexOptions.IgnoreCase, MatchTimeout);

        //  حذف متن خارج از بلاک تحلیلی (optional ولی مهم)
        text = TrimToRelevantContent(text);

        return text.Trim();
    }

    private static string NormalizeTerminology(string text)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [key: "Cloud Computing"] = "A.",
            [key: "Machine Learning"] = "B.",
            [key: "Open Source"] = "C.",
            [key: "Data Structure"] = "D."
        };

        foreach (var (key, prefix) in map)
        {
            text = Regex.Replace(text, $@"^{key}\s*[:\-]\s*(.+)$", $"{prefix} $1",
                RegexOptions.Multiline | RegexOptions.IgnoreCase, MatchTimeout);
        }

        return text;
    }

    private static string TrimToRelevantContent(string text)
    {
        var knownLabels = SectionLabels.Concat(TerminologyLabels).Append(ConfidenceLabel).Select(Regex.Escape);
        var pattern = $@"(?:^|\n)({string.Join(separator: '|', knownLabels)})";
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase, MatchTimeout);

        return match.Success ? text[match.Index..] : text;
    }
}
