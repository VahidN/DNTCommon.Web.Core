using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class ScheduledTasksController : Controller
{
    private readonly IOptions<ScheduledTasksStorage> _tasksStorage;

    public ScheduledTasksController(IOptions<ScheduledTasksStorage> tasksStorage)
    {
        _tasksStorage = tasksStorage;
    }

    public IActionResult Index()
    {
        return View(model: _tasksStorage.Value.Tasks);
    }
}