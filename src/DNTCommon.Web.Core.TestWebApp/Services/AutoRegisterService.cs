using System;

namespace DNTCommon.Web.Core.TestWebApp.Services;

public interface IAutoRegisterService : ISingletonService
{
    string GetDate();
}

public class AutoRegisterService : IAutoRegisterService
{
    public string GetDate() => DateTime.UtcNow.ToShortDateString();
}
