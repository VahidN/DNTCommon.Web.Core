using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core.TestWebApp.Models
{
    public class UserFileViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        //`FileExtensions` needs to be applied to a string property. It doesn't work on IFormFile properties, and definitely not on IEnumerable<IFormFile> properties.
        //[FileExtensions(Extensions = ".png,.jpg,.jpeg,.gif", ErrorMessage = "Please upload an image file.")]
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "Please upload an image file.")]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
    }
}