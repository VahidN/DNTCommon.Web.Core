using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class SyndicationRssReaderTests : TestsBase
{
    [TestMethod]
    public async Task ReadAsyncShouldParseRssFeedCorrectly()
    {
        // Arrange
        var rssXml = """
                     <?xml version="1.0" encoding="utf-8"?>
                     <rss version="2.0">
                       <channel>
                         <title>Sample RSS Feed</title>
                         <description>This is a test RSS feed</description>
                         <copyright>Copyright 2025</copyright>
                         <language>en-US</language>
                         <ttl>30</ttl>

                         <item>
                           <title>First Item</title>
                           <link>https://example.com/item1</link>
                           <description><![CDATA[Item 1 description]]></description>
                           <author>john.doe@example.com (John Doe)</author>
                           <category>Tech</category>
                           <category>.NET</category>
                           <pubDate>Wed, 07 Oct 2009 10:00:00 GMT</pubDate>
                           <guid>1</guid>
                         </item>
                       </channel>
                     </rss>
                     """;

        var handler = new FakeHttpMessageHandler(rssXml);
        var httpClient = new HttpClient(handler);

        // Act
        var feed = await httpClient.ReadRssAsync(url: "https://fake-url/rss");

        // Assert (Feed)
        Assert.AreEqual(expected: "Sample RSS Feed", feed.FeedTitle);
        Assert.AreEqual(expected: "This is a test RSS feed", feed.FeedDescription);
        Assert.AreEqual(expected: "Copyright 2025", feed.FeedCopyright);
        Assert.AreEqual(expected: "en-US", feed.CultureName);
        Assert.AreEqual(TimeSpan.FromMinutes(minutes: 30), feed.TimeToLive);

        // Assert (Items)
        var item = feed.RssItems!.First();

        Assert.AreEqual(expected: "First Item", item.Title);
        Assert.AreEqual(expected: "john.doe@example.com (John Doe)", item.AuthorName);
        Assert.AreEqual(expected: "Item 1 description", item.Content);
        Assert.AreEqual(expected: "https://example.com/item1", item.Url);

        CollectionAssert.Contains(item.Categories.ToList(), element: "Tech");
        CollectionAssert.Contains(item.Categories.ToList(), element: ".NET");

        Assert.AreEqual(
            new DateTimeOffset(year: 2009, month: 10, day: 7, hour: 10, minute: 0, second: 0, TimeSpan.Zero),
            item.PublishDate);
    }
}
