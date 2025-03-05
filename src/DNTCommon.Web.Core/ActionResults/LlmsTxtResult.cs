using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core llms.txt provider.
///     https://llmstxt.org/
/// </summary>
public class LlmsTxtResult<TFeedItem>(FeedChannel<TFeedItem> feedChannel) : ActionResult
    where TFeedItem : FeedItem
{
    /// <summary>
    ///     Executes the result operation of the action method asynchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;

        response.ContentType = new MediaTypeHeaderValue(mediaType: "text/plain")
        {
            CharSet = Encoding.UTF8.WebName
        }.ToString();

        var data = GetLlmsTxtData();

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }

    private byte[] GetLlmsTxtData()
    {
        var builder = new StringBuilder();
        builder.Append(value: "# ").AppendLine(feedChannel.FeedTitle).AppendLine();
        builder.AppendLine(value: "## Docs").AppendLine();

        if (feedChannel.RssItems is not null)
        {
            builder.AppendLine(value: "Blog posts:").AppendLine();

            foreach (var item in feedChannel.RssItems)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, format: " - [{0}]({1})", item.Title, item.Url)
                    .AppendLine();
            }
        }

        return builder.ToString().ToBytes()!;
    }
}
