using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class BackgroundQueueController : Controller
{
    private readonly IBackgroundQueueService _queue;

    public BackgroundQueueController(IBackgroundQueueService queue) => _queue = queue;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Synchronous version
        await _queue.QueueBackgroundWorkItemAsync(nameof(BackgroundQueueController),
            (cancellationToken, serviceProvider) =>
            {
                //NOTES:
                // 1- There is no `HttpContext` available here.
                // 2- Retrive your services from the `serviceProvider` using the `serviceProvider.GetRequiredService<T>()` method.
                //    Because other injected services will be disposed at the end of the request automatically.

                Console.WriteLine(value: "Running job 1");

                var emailService = serviceProvider.GetRequiredService<IWebMailService>();

                // ...
            }, ct);

        // Asynchronous version
        await _queue.QueueBackgroundWorkItemAsync(nameof(BackgroundQueueController),
            async (cancellationToken, serviceProvider) =>
            {
                await Task.Delay(millisecondsDelay: 1000, cancellationToken);
                Console.WriteLine(value: "Running job 2");
            }, ct);

        return View();
    }
}
