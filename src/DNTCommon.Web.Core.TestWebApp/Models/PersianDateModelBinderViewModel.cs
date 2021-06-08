using System;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Models
{
    // NOTE! [PersianDateModelBinder] doesn't work in .NET 5x anymore! Don't use it!
    // [ModelBinder(BinderType = typeof(PersianDateModelBinder))]
    public class PersianDateModelBinderViewModel
    {
        public DateTime PDateTime1 { set; get; }
        public DateTime GDateTime1 { set; get; }

        public DateTime? PDateTime2 { set; get; }
        public DateTime? GDateTime2 { set; get; }
    }
}