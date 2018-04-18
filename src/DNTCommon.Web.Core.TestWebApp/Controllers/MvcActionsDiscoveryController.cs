using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class MvcActionsDiscoveryController : Controller
    {
        private readonly IMvcActionsDiscoveryService _mvcActionsDiscoveryService;

        public MvcActionsDiscoveryController(IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _mvcActionsDiscoveryService = mvcActionsDiscoveryService;
        }

        public IActionResult Index()
        {
            //var securedControllerActions = _mvcActionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy("DynamicPermission");
            var controllerActions = _mvcActionsDiscoveryService.MvcControllers;
            return View(controllerActions);
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(string[] actionIds)
        {
            return Json(actionIds);
        }
    }
}