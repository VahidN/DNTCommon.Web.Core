using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class DesCryptoProviderController : Controller
    {
        private readonly IDesCryptoProvider _protectionProviderService;

        public DesCryptoProviderController(IDesCryptoProvider protectionProviderService)
        {
            _protectionProviderService = protectionProviderService;
        }

        public IActionResult Index()
        {
            ViewBag.Msg = "This is a test";
            const string key = "this is my secure key!";

            ViewBag.EncryptedMsg1 = _protectionProviderService.Encrypt(ViewBag.Msg, key);
            ViewBag.DecryptMsg1 = _protectionProviderService.Decrypt(ViewBag.EncryptedMsg1, key);

            ViewBag.EncryptedMsg2 = _protectionProviderService.Encrypt(ViewBag.Msg, key);
            ViewBag.DecryptMsg2 = _protectionProviderService.Decrypt(ViewBag.EncryptedMsg2, key);

            var model = new TestModel { Id = 1, Name = "Test" };
            ViewBag.EncryptedMsg3 = _protectionProviderService.EncryptObject(model, key);
            ViewBag.DecryptMsg3 = _protectionProviderService.DecryptObject<TestModel>(ViewBag.EncryptedMsg3, key).Name;

            return View();
        }
    }
}