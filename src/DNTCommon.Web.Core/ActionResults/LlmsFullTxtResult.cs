using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core `llms-full.txt` provider.
///     It's a simple text file format for sharing a full-text feed.
///     https://llmstxt.org/
/// </summary>
/// <typeparam name="TFeedItem">The type of the feed item, which must inherit from <see cref="FeedItem"/>.</typeparam>
/// <param name="feedChannel">The feed channel containing the data to be formatted as llms-full.txt.</param>
/// <remarks>
///     This class is responsible for generating a `llms-full.txt` response from a given <see cref="FeedChannel{TFeedItem}"/>.
///     It iterates through the <see cref="FeedItem"/>s in the channel, formats their title, URL, and content,
///     and writes the result to the HTTP response body.
/// </remarks>
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
