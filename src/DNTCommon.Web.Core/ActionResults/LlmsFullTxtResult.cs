using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core llms-full.txt provider.
///     https://llmstxt.org/
/// </summary>
public class LlmsFullTxtResult<TFeedItem>(FeedChannel<TFeedItem> feedChannel) : ActionResult
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

        var data = GetLlmsFullTxtData();

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }

    private byte[] GetLlmsFullTxtData()
    {
        var builder = new StringBuilder();

        if (feedChannel.RssItems is not null)
        {
            foreach (var item in feedChannel.RssItems)
            {
                if (item.Content.IsEmpty())
                {
                    continue;
                }

                var content = item.Content.Trim();

                if (content.StartsWith(value: "---", StringComparison.OrdinalIgnoreCase))
                {
                    content = content.TrimStart(value: "---", StringComparison.OrdinalIgnoreCase);
                }

                builder.Append(value: "# ").AppendLine(item.Title);
                builder.Append(value: "Source: ").AppendLine(item.Url).AppendLine();

                builder.AppendLine(content).AppendLine();
            }
        }

        return builder.ToString().ToBytes()!;
    }
}
