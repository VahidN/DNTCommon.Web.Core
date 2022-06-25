using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Models;

//[ModelBinder(BinderType = typeof(YeKeModelBinder))]
public class YeKeModelBinderViewModel
{
    public string Data { set; get; }
}