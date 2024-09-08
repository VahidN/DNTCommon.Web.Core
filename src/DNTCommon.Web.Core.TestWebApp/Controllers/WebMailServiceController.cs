using System.Collections.Generic;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class WebMailServiceController : Controller
{
    private readonly IWebMailService _webMailService;
    private SmtpConfig _smtpConfig;

    public WebMailServiceController(IWebMailService webMailService,
        IOptionsMonitor<SmtpConfig> smtpConfigMonitor // will be provided from the `appsettings.json` file.
    )
    {
        _webMailService = webMailService;
        _smtpConfig = smtpConfigMonitor.CurrentValue;
        smtpConfigMonitor.OnChange(option => { _smtpConfig = option; });
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> SendEmail()
    {
        await _webMailService.SendEmailAsync(_smtpConfig, new List<MailAddress>
        {
            new()
            {
                ToName = "User 1",
                ToAddress = "user1@site.com"
            }

            // ...
        }, subject: "Hello!", message: "Hello!<br/> This is an email from us!");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SendRazorTemplateEmail()
    {
        await _webMailService.SendEmailAsync(_smtpConfig, new List<MailAddress>
            {
                new()
                {
                    ToName = "User 1",
                    ToAddress = "user1@site.com"
                }

                // ...
            }, subject: "Please verify your account", viewNameOrPath: "~/Views/EmailTemplates/_Template1.cshtml",
            new EmailTemplateViewModel
            {
                EmailSignature = "DNT"
            });

        return RedirectToAction(nameof(Index));
    }
}