using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class AllowUploadSafeFilesController : Controller
    {
        private readonly IUploadFileService _uploadFileService;

        public AllowUploadSafeFilesController(IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadGeneralFile(GeneralFileViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var formFile = userViewModel.UserFile;
                if (!await _uploadFileService.SavePostedFileAsync(formFile, destinationDirectoryName: "files", allowOverwrite: false))
                {
                    ModelState.AddModelError("", "Uploaded file is null or empty.");
                    return View(viewName: "Index");
                }

                RedirectToAction("Index");
            }
            return View(viewName: "Index");
        }
    }
}