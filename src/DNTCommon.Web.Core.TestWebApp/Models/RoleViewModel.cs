using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Models
{
    public class RoleViewModel
    {
        [HiddenInput]
        public int Id { set; get; }

        [Required(ErrorMessage = "(*)")]
        [Display(Name = "نام نقش")]
        public string Name { set; get; }
    }
}