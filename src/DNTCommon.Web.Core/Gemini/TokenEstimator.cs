namespace DNTCommon.Web.Core;

public static class TokenEstimator
{
    private static readonly char[] Separator = [' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?'];

    public static int EstimateMaxOutputTokens(string inputText,
        double tokensPerWordRatio = 0.75,
        double safetyFactor = 1.5)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            return 50; // مقدار پیش فرض برای متن خالی
        }

        // تقسیم متن به کلمات (می توانید از روش های پیشرفته تر برای توکن سازی استفاده کنید)
        var words = inputText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        var wordCount = words.Length;

        // تخمین تعداد توکن ها بر اساس تعداد کلمات و نسبت تقریبی
        var estimatedTokens = wordCount * tokensPerWordRatio;

        // اضافه کردن یک فاکتور ایمنی برای اطمینان از کافی بودن توکن ها برای پاسخ
        var maxOutputTokens = (int)Math.Ceiling(estimatedTokens * safetyFactor);

        // اطمینان از اینکه مقدار تخمین زده شده خیلی کم نباشد
        return Math.Max(val1: 50, maxOutputTokens); // حداقل 50 توکن
    }
}
