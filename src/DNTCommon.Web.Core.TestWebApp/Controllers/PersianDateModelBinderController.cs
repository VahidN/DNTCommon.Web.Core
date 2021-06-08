using System;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class PersianDateModelBinderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(
               //[FromBody]
               // Note:
               // The [FromBody] attribute means directing the default behavior of Model Binding to use
               // the formatters/InputFormatter instead. In this case our custom Model Binder does not work.
               // To use our custom model binder, remove the [FromBody] Attribute.
               // And now he default value providers get data from a form-URL-encoded body, route values, and the query string.
               [FromForm] PersianDateModelBinderViewModel model)
        {
            throw new NotSupportedException("[PersianDateModelBinder] doesn't work in .NET 5x anymore! Don't use it!");
            return Json(model);
        }
    }
}