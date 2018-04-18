using System.IO;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class UploadFileExtensionsController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public UploadFileExtensionsController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(UserFileViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var formFile = userViewModel.Photo;
                if (formFile == null || formFile.Length == 0)
                {
                    ModelState.AddModelError("", "Uploaded file is empty or null.");
                    return View(viewName: "Index");
                }

                var uploadsRootFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                var filePath = Path.Combine(uploadsRootFolder, formFile.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream).ConfigureAwait(false);
                }

                RedirectToAction("Index");
            }
            return View(viewName: "Index");
        }

    }
}