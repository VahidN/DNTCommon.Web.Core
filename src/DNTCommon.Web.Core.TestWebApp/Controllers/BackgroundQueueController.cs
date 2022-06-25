using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class BackgroundQueueController : Controller
{
    private readonly IBackgroundQueueService _queue;

    public BackgroundQueueController(IBackgroundQueueService queue)
    {
        _queue = queue;
    }

    public IActionResult Index()
    {
        // Synchronous version
        _queue.QueueBackgroundWorkItem((cancellationToken, serviceProvider) =>
        {
            //NOTES:
            // 1- There is no `HttpContext` available here.
            // 2- Retrive your services from the `serviceProvider` using the `serviceProvider.GetRequiredService<T>()` method.
            //    Because other injected services will be disposed at the end of the request automatically.

            Console.WriteLine("Running job 1");

            var emailService = serviceProvider.GetRequiredService<IWebMailService>();
            // ...
        });

        // Asynchronous version
        _queue.QueueBackgroundWorkItem(async (cancellationToken, serviceProvider) =>
        {
            await Task.Delay(1000);
            Console.WriteLine("Running job 2");
        });
        return View();
    }
}