using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class WebMailServiceController : Controller
    {
        private readonly IWebMailService _webMailService;
        private readonly IOptionsSnapshot<SmtpConfig> _smtpConfig;

        public WebMailServiceController(
            IWebMailService webMailService,
            IOptionsSnapshot<SmtpConfig> smtpConfig // will be provided from the `appsettings.json` file.
            )
        {
            _webMailService = webMailService;
            _smtpConfig = smtpConfig;

            _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(smtpConfig));
            if (_smtpConfig.Value == null)
            {
                throw new ArgumentNullException(nameof(smtpConfig), "Please add SmtpConfig to your appsettings.json file.");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SendEmail()
        {
            await _webMailService.SendEmailAsync(
                           smtpConfig: _smtpConfig.Value,
                           emails: new List<MailAddress>
                           {
                                new MailAddress { ToName = "User 1", ToAddress = "user1@site.com" },
                                // ...
                           },
                           subject: "Hello!",
                           message: "Hello!<br/> This is an email from us!");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SendRazorTemplateEmail()
        {
            await _webMailService.SendEmailAsync(
                           smtpConfig: _smtpConfig.Value,
                           emails: new List<MailAddress>
                           {
                                new MailAddress { ToName = "User 1", ToAddress = "user1@site.com" },
                                // ...
                           },
                           subject: "Please verify your account",
                           viewNameOrPath: "~/Views/EmailTemplates/_Template1.cshtml",
                           viewModel: new EmailTemplateViewModel
                           {
                               EmailSignature = "DNT"
                           });
            return RedirectToAction(nameof(Index));
        }
    }
}