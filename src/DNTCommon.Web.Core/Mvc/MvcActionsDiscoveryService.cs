using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using System.ComponentModel.DataAnnotations;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// MvcActions Discovery Service
    /// </summary>
    public class MvcActionsDiscoveryService : IMvcActionsDiscoveryService
    {
        // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
        // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
        // threads but only one of the objects succeeds in creating a GetterInfo.
        private readonly ConcurrentDictionary<string, Lazy<ICollection<MvcControllerViewModel>>> _allSecuredActionsWithPloicy = new(StringComparer.Ordinal);

        /// <summary>
        /// MvcActions Discovery Service
        /// </summary>
        public MvcActionsDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (actionDescriptorCollectionProvider == null)
            {
                throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
            }

            MvcControllers = new List<MvcControllerViewModel>();

            var lastControllerName = string.Empty;
            MvcControllerViewModel? currentController = null;

            var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (actionDescriptor is not ControllerActionDescriptor descriptor)
                {
                    continue;
                }

                var controllerTypeInfo = descriptor.ControllerTypeInfo;
                var actionMethodInfo = descriptor.MethodInfo;

                if (!lastControllerName.Equals(descriptor.ControllerName, StringComparison.Ordinal))
                {
                    currentController = new MvcControllerViewModel
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        ControllerDisplayName =
                           controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                           controllerTypeInfo.GetCustomAttribute<DisplayAttribute>()?.Name,
                        ControllerName = descriptor.ControllerName,
                    };
                    currentController.ControllerAttributes.AddRange(getAttributes(controllerTypeInfo));
                    MvcControllers.Add(currentController);

                    lastControllerName = descriptor.ControllerName;
                }

                if (currentController == null)
                {
                    continue;
                }

                var mvcActionItem = new MvcActionViewModel
                {
                    ControllerId = currentController.ControllerId,
                    ActionName = descriptor.ActionName,
                    ActionDisplayName =
                                      actionMethodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                                      actionMethodInfo.GetCustomAttribute<DisplayAttribute>()?.Name,
                    IsSecuredAction = isSecuredAction(controllerTypeInfo, actionMethodInfo)
                };
                mvcActionItem.ActionAttributes.AddRange(getAttributes(actionMethodInfo));
                currentController.MvcActions.Add(mvcActionItem);
            }
        }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application.
        /// </summary>
        public ICollection<MvcControllerViewModel> MvcControllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        public ICollection<MvcControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName)
        {
            var getter = _allSecuredActionsWithPloicy.GetOrAdd(policyName, y => new Lazy<ICollection<MvcControllerViewModel>>(
                () =>
                {
                    var controllers = new List<MvcControllerViewModel>(MvcControllers);
                    foreach (var controller in controllers)
                    {
						var securedOnes = controller.MvcActions.Where(
											model => model.IsSecuredAction &&
											(
												string.Equals(model.ActionAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy, policyName, StringComparison.Ordinal) ||
												string.Equals(controller.ControllerAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy, policyName, StringComparison.Ordinal)
											)).ToList();										
						controller.MvcActions.Clear();
                        controller.MvcActions.AddRange(securedOnes);                            
                    }
                    return controllers.Where(model => model.MvcActions.Any()).ToList();
                }));
            return getter.Value;
        }

        private static List<Attribute> getAttributes(MemberInfo actionMethodInfo)
        {
            return actionMethodInfo.GetCustomAttributes(inherit: true)
                                   .Where(attribute =>
                                    {
                                        var attributeNamespace = attribute.GetType().Namespace;
                                        return !string.Equals(attributeNamespace, typeof(CompilerGeneratedAttribute).Namespace, StringComparison.Ordinal) &&
                                            !string.Equals(attributeNamespace, typeof(DebuggerStepThroughAttribute).Namespace, StringComparison.Ordinal);
                                    })
                                    .Cast<Attribute>()
                                   .ToList();
        }

        private static bool isSecuredAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            var actionHasAllowAnonymousAttribute = actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(inherit: true) != null;
            if (actionHasAllowAnonymousAttribute)
            {
                return false;
            }

            var controllerHasAuthorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            if (controllerHasAuthorizeAttribute)
            {
                return true;
            }

            var actionMethodHasAuthorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            if (actionMethodHasAuthorizeAttribute)
            {
                return true;
            }

            return false;
        }
    }
}