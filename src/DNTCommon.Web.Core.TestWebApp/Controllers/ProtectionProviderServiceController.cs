using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class ProtectionProviderServiceController : Controller
    {
        private readonly IProtectionProviderService _protectionProviderService;

        public ProtectionProviderServiceController(IProtectionProviderService protectionProviderService)
        {
            _protectionProviderService = protectionProviderService;
        }

        public IActionResult Index()
        {
            ViewBag.Msg = "This is a test";
            ViewBag.EncryptedMsg = _protectionProviderService.Encrypt(ViewBag.Msg);
            ViewBag.DecryptMsg = _protectionProviderService.Decrypt(ViewBag.EncryptedMsg);
            return View();
        }
    }
}