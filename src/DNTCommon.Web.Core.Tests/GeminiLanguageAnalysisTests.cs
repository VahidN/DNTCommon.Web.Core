using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class GeminiLanguageAnalysisTests : TestsBase
{
    [TestMethod]
    public void Parsed_Values_Should_Not_Contain_Any_Label()
    {
        var input = """
                    1. Translation Result:
                    این جمله اول است.
                    این جمله دوم است.

                    2. Explanation Result:
                    توضیح اینجاست

                    A. رایانش ابری
                    B. یادگیری ماشین
                    C. متن‌باز
                    D. ساختار داده

                    CONFIDENCE RATING (1-10): 9
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);
            Assert.IsNotNull(result);

            string[] allValues =
            [
                result.Translation, result.Explanation, result.CloudComputing, result.MachineLearning,
                result.OpenSource, result.DataStructure
            ];

            var forbiddenTokens = GeminiLanguageAnalysisService.SectionLabels
                .Concat(GeminiLanguageAnalysisService.TerminologyLabels)
                .Append(GeminiLanguageAnalysisService.ConfidenceLabel)
                .ToList();

            foreach (var value in allValues)
            {
                foreach (var token in forbiddenTokens)
                {
                    Assert.IsFalse(value.Contains(token, StringComparison.OrdinalIgnoreCase),
                        $"Value incorrectly contains label '{token}': {value}");
                }
            }
        });
    }

    [TestMethod]
    public void Parse_WellFormed_Output_Should_Succeed()
    {
        var input = """
                    1. Translation Result:
                    این جمله اول است.
                    این جمله دوم است.

                    2. Explanation Result:
                    داکر یک ابزار برای مدیریت کانتینرهاست.

                    A. رایانش ابری
                    B. یادگیری ماشین
                    C. متن‌باز
                    D. ساختار داده

                    CONFIDENCE RATING (1-10): 8
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "این جمله اول است.\nاین جمله دوم است.", result.Translation);
            Assert.AreEqual(expected: "داکر یک ابزار برای مدیریت کانتینرهاست.", result.Explanation);
            Assert.AreEqual(expected: "رایانش ابری", result.CloudComputing);
            Assert.AreEqual(expected: "یادگیری ماشین", result.MachineLearning);
            Assert.AreEqual(expected: "متن‌باز", result.OpenSource);
            Assert.AreEqual(expected: "ساختار داده", result.DataStructure);
            Assert.AreEqual(expected: 8, result.ConfidenceRating);
        });
    }

    [TestMethod]
    public void AutoRepair_Should_Fix_SingleLine_Labels()
    {
        var input = """
                    1. Translation Result: این جمله اول است. این جمله دوم است.
                    2. Explanation Result: داکر ابزار کانتینرسازی است.
                    Confidence = 9
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Translation.StartsWith(value: "این جمله اول است", StringComparison.Ordinal));
            Assert.IsTrue(result.Explanation.Contains(value: "داکر", StringComparison.Ordinal));
            Assert.AreEqual(expected: 9, result.ConfidenceRating);
        });
    }

    [TestMethod]
    public void AutoRepair_Should_Normalize_Terminology_Without_Letters()
    {
        var input = """
                    Translation Result:
                    تست

                    Explanation Result:
                    تست

                    Cloud Computing: رایانش ابری
                    Machine Learning - یادگیری ماشین
                    Open Source: متن باز
                    Data Structure: ساختار داده

                    CONFIDENCE RATING (1-10): 7
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "رایانش ابری", result.CloudComputing);
            Assert.AreEqual(expected: "یادگیری ماشین", result.MachineLearning);
            Assert.AreEqual(expected: "متن باز", result.OpenSource);
            Assert.AreEqual(expected: "ساختار داده", result.DataStructure);
        });
    }

    [TestMethod]
    public void Parser_Should_Handle_Shuffled_Order()
    {
        var input = """
                    CONFIDENCE RATING (1-10): 6

                    B. یادگیری ماشین
                    D. ساختار داده

                    2. Explanation Result:
                    توضیح اینجاست

                    A. رایانش ابری
                    C. متن‌باز

                    1. Translation Result:
                    ترجمه اینجاست
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "ترجمه اینجاست", result.Translation);
            Assert.AreEqual(expected: "توضیح اینجاست", result.Explanation);
            Assert.AreEqual(expected: "رایانش ابری", result.CloudComputing);
            Assert.AreEqual(expected: "یادگیری ماشین", result.MachineLearning);
            Assert.AreEqual(expected: "متن‌باز", result.OpenSource);
            Assert.AreEqual(expected: "ساختار داده", result.DataStructure);
            Assert.AreEqual(expected: 6, result.ConfidenceRating);
        });
    }

    [TestMethod]
    public void Missing_Sections_Should_Not_Throw()
    {
        var input = """
                    1. Translation Result:
                    فقط ترجمه موجود است
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "فقط ترجمه موجود است", result.Translation);
            Assert.AreEqual(string.Empty, result.Explanation);
            Assert.IsNull(result.ConfidenceRating);
        });
    }

    [TestMethod]
    public void Garbage_Text_Before_Template_Should_Be_Ignored()
    {
        var input = """
                    Hello! Here is your result:

                    1. Translation Result:
                    ترجمه سالم

                    CONFIDENCE RATING (1-10): 10
                    """;

        ServiceProvider.RunScopedService<IGeminiLanguageAnalysisService>(geminiLanguageAnalysisService =>
        {
            var result = geminiLanguageAnalysisService.GetGeminiPersianLanguageAnalysisResult(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "ترجمه سالم", result.Translation);
            Assert.AreEqual(expected: 10, result.ConfidenceRating);
        });
    }
}
