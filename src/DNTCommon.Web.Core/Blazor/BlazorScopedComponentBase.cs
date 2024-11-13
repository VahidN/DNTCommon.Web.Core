using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Some services such as unit of work/DbContext are unique. Because we want them to last the duration of the
///     component, and we don't want them to be shared across different components to avoid exceptions like:
///     `A second operation started on this context before a previous operation completed`
///     Because Entity Framework Core does not support multiple parallel operations (such as parallel rendering of SSR
///     components) being run on the same DbContext instance.
///     `OwningComponentBase` provides a special scope that overrides the default behavior and provides the instance for
///     the duration of the component. This means the same user will get a new copy when they return to that component, and
///     the service is properly disposed of when the component goes out of scope.
///     Usage:
///     Add this line to your `_Imports.razor` file:
///     @inherits BlazorScopedComponentBase
///     And then define your scoped services like this in components:
///     [InjectComponentScoped] internal UsersService MyUsersService { set; get; } = null!;
///     Note:
///     You shouldn't use `@inject UsersService MyUsersService` anymore! These types of scoped services will be shared and
///     kept alive during the lifetime of the components.
///     Also, if you are overriding the `OnInitialized()` method of your component, make sure to call the
///     `base.OnInitialized();` method first. Otherwise, initialization of your `[InjectComponentScoped]` properties and
///     services will not happen.
/// </summary>
public class BlazorScopedComponentBase : OwningComponentBase
{
    // 'GetOrAdd' call on the dictionary is not thread safe, and we might end up creating the list, more than
    // once. To prevent this, Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
    // threads but only one of the objects succeeds in creating the list.
    private static readonly ConcurrentDictionary<Type, Lazy<List<PropertyInfo>>> CachedProperties = new();

    private List<PropertyInfo> InjectComponentScopedPropertiesList => CachedProperties.GetOrAdd(GetType(),
            type => new Lazy<List<PropertyInfo>>(
                () => type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                         BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<InjectComponentScopedAttribute>() is not null)
                    .ToList(), LazyThreadSafetyMode.ExecutionAndPublication))
        .Value;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        foreach (var propertyInfo in InjectComponentScopedPropertiesList)
        {
            propertyInfo.SetValue(this, ScopedServices.GetRequiredService(propertyInfo.PropertyType));
        }
    }
}