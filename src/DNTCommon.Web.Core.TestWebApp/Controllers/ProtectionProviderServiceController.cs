using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class TestModel
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

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

            ViewBag.EncryptedMsg1 = _protectionProviderService.Encrypt(ViewBag.Msg);
            ViewBag.DecryptMsg1 = _protectionProviderService.Decrypt(ViewBag.EncryptedMsg1);

            ViewBag.EncryptedMsg2 = _protectionProviderService.Encrypt(ViewBag.Msg);
            ViewBag.DecryptMsg2 = _protectionProviderService.Decrypt(ViewBag.EncryptedMsg2);

            var model = new TestModel { Id = 1, Name = "Test" };
            ViewBag.EncryptedMsg3 = _protectionProviderService.EncryptObject(model);
            ViewBag.DecryptMsg3 = _protectionProviderService.DecryptObject<TestModel>(ViewBag.EncryptedMsg3).Name;

            return View();
        }
    }
}