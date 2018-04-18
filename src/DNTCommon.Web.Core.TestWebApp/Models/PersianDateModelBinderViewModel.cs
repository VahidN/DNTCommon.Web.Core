using System;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Models
{
    [ModelBinder(BinderType = typeof(PersianDateModelBinder))]
    public class PersianDateModelBinderViewModel
    {
        public DateTime PDateTime1 { set; get; }
        public DateTime GDateTime1 { set; get; }

        public DateTime? PDateTime2 { set; get; }
        public DateTime? GDateTime2 { set; get; }
    }
}