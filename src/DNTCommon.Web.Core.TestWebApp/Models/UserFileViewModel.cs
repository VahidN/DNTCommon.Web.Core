using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core.TestWebApp.Models;

public class UserFileViewModel
{
    [Required(ErrorMessage = "Please select a file.")]

    //`FileExtensions` needs to be applied to a string property. It doesn't work on IFormFile properties, and definitely not on IEnumerable<IFormFile> properties.
    //[FileExtensions(Extensions = ".png,.jpg,.jpeg,.gif", ErrorMessage = "Please upload an image file.")]
    [UploadFileExtensions(FileExtensions = ".png,.jpg,.jpeg,.gif", ErrorMessage = "Please upload an image file.")]
    [DataType(DataType.Upload)]
    public IFormFile Photo { get; set; }
}

public class GeneralFileViewModel
{
    [Required(ErrorMessage = "Please select a file.")]
    [AllowUploadSafeFiles(ErrorMessage = "You are not allowed to upload these types of files.")]
    [DataType(DataType.Upload)]
    public IFormFile UserFile { get; set; }
}
