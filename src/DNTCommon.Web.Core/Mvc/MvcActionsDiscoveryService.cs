using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DNTCommon.Web.Core;

/// <summary>
///     MvcActions Discovery Service
/// </summary>
public class MvcActionsDiscoveryService : IMvcActionsDiscoveryService
{
    // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
    // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
    // threads but only one of the objects succeeds in creating a GetterInfo.
    private readonly ConcurrentDictionary<string, Lazy<ICollection<MvcControllerViewModel>>>
        _allSecuredActionsWithPloicy = new(StringComparer.Ordinal);

    /// <summary>
    ///     MvcActions Discovery Service
    /// </summary>
    public MvcActionsDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        if (actionDescriptorCollectionProvider == null)
        {
            throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
        }

        var apiControllers = new HashSet<MvcControllerViewModel>(new MvcControllerEqualityComparer());

        foreach (var descriptor in actionDescriptorCollectionProvider.ActionDescriptors.Items
                                                                     .OfType<ControllerActionDescriptor>())
        {
            var controllerTypeInfo = descriptor.ControllerTypeInfo;
            var actionMethodInfo = descriptor.MethodInfo;

            var controllerAttributes = controllerTypeInfo.GetCustomAttributes(true);
            var currentController = new MvcControllerViewModel
                                    {
                                        AreaName = controllerAttributes.OfType<AreaAttribute>().FirstOrDefault()
                                                                       ?.RouteValue ?? string.Empty,
                                        ControllerDisplayName = GetDisplayName(controllerAttributes),
                                        ControllerName = descriptor.ControllerName,
                                        ControllerAttributes = getAttributes(controllerTypeInfo),
                                    };
            if (apiControllers.TryGetValue(currentController, out var actualController))
            {
                currentController = actualController;
            }
            else
            {
                apiControllers.Add(currentController);
            }

            var actionMethodAttributes = actionMethodInfo.GetCustomAttributes(true);
            var mvcActionItem = new MvcActionViewModel
                                {
                                    ControllerId = currentController.ControllerId,
                                    ActionName = descriptor.ActionName,
                                    ActionDisplayName = GetDisplayName(actionMethodAttributes),
                                    IsSecuredAction = isSecuredAction(controllerTypeInfo, actionMethodInfo),
                                    HttpMethods =
                                        descriptor.ActionConstraints?.OfType<HttpMethodActionConstraint>()
                                                  .FirstOrDefault()?.HttpMethods ??
                                        new[] { "any" },
                                    ActionAttributes = getAttributes(actionMethodInfo),
                                };
            currentController.MvcActions.Add(mvcActionItem);
        }

        MvcControllers = apiControllers;
    }

    /// <summary>
    ///     Returns the list of all of the controllers and action methods of an MVC application.
    /// </summary>
    public ICollection<MvcControllerViewModel> MvcControllers { get; }

    /// <summary>
    ///     Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute
    ///     and the specified policyName.
    /// </summary>
    public ICollection<MvcControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName)
    {
        var getter =
            _allSecuredActionsWithPloicy.GetOrAdd(policyName,
                                                  pn => new Lazy<ICollection<MvcControllerViewModel>>(
                                                       () =>
                                                       {
                                                           var controllers =
                                                               new List<
                                                                   MvcControllerViewModel>(MvcControllers);
                                                           foreach (var controller in controllers)
                                                           {
                                                               var securedOnes = controller.MvcActions.Where(
                                                                    model => model.IsSecuredAction &&
                                                                             (
                                                                                 string.Equals(model
                                                                                          .ActionAttributes
                                                                                          .OfType<
                                                                                              AuthorizeAttribute>()
                                                                                          .FirstOrDefault()?.Policy,
                                                                                      pn,
                                                                                      StringComparison.Ordinal) ||
                                                                                 string.Equals(controller
                                                                                          .ControllerAttributes
                                                                                          .OfType<
                                                                                              AuthorizeAttribute>()
                                                                                          .FirstOrDefault()?.Policy,
                                                                                      pn,
                                                                                      StringComparison.Ordinal)
                                                                             )).ToList();
                                                               controller.MvcActions.Clear();
                                                               controller.MvcActions.AddRange(securedOnes);
                                                           }

                                                           return controllers
                                                                  .Where(model => model.MvcActions.Any())
                                                                  .ToList();
                                                       }));
        return getter.Value;
    }

    private static string GetDisplayName(object[] attributes) =>
        attributes.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ??
        attributes.OfType<DisplayAttribute>().FirstOrDefault()?.Name ?? string.Empty;

    private static List<Attribute> getAttributes(MemberInfo actionMethodInfo)
    {
        return actionMethodInfo.GetCustomAttributes(true)
                               .Where(attribute =>
                                      {
                                          var attributeNamespace = attribute.GetType().Namespace;
                                          return !string.Equals(attributeNamespace,
                                                                typeof(CompilerGeneratedAttribute).Namespace,
                                                                StringComparison.Ordinal) &&
                                                 !string.Equals(attributeNamespace,
                                                                typeof(DebuggerStepThroughAttribute).Namespace,
                                                                StringComparison.Ordinal);
                                      })
                               .Cast<Attribute>()
                               .ToList();
    }

    private static bool isSecuredAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
    {
        var actionHasAllowAnonymousAttribute =
            actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true) != null;
        if (actionHasAllowAnonymousAttribute)
        {
            return false;
        }

        var controllerHasAuthorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null;
        if (controllerHasAuthorizeAttribute)
        {
            return true;
        }

        var actionMethodHasAuthorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null;
        if (actionMethodHasAuthorizeAttribute)
        {
            return true;
        }

        return false;
    }
}