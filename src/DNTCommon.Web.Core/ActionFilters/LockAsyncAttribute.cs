using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Applies a lock to an asynchronous action method, ensuring that only
///     one instance of the method executes at a time.
///     TKey is a key to lock on. You can select the controller's type here.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LockAsyncAttribute<TKey> : Attribute, IAsyncActionFilter
    where TKey : Type
{
    /// <summary>
    ///     A TimeSpan that represents the number of milliseconds to wait.
    ///     Its default value is 30 seconds.
    /// </summary>
    public TimeSpan LockTimeout { set; get; } = TimeSpan.FromSeconds(value: 30);

    /// <summary>
    ///     Applies a lock to an asynchronous action method, ensuring that only
    ///     one instance of the method executes at a time.
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var lockerService = context.HttpContext.RequestServices.GetRequiredService<ILockerService>();
        using var locker = await lockerService.LockAsync<TKey>(LockTimeout);
        await next();
    }
}
