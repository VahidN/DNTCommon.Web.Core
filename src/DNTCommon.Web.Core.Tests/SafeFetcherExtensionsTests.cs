using System.Net.Http;
using System.Threading.Tasks;
using DNTCommon.Web.Core;
using DNTCommon.Web.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SafeFetcherExtensionsTests : TestsBase
{
    [TestMethod]
    public async Task Test1SafeFetchAsyncWorks()
        => await ServiceProvider.RunScopedServiceAsync<IHttpClientFactory>(async (httpClientFactory, ct) =>
        {
            using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
            var result = await client.SafeFetchAsync(uri: "https://petabridge.com/blog/ai-wont-kill-open-source", ct);

            Assert.AreEqual(FetchResultKind.Success, result.Kind);
        });
}
