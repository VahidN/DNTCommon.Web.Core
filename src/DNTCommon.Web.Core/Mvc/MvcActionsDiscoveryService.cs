using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
    /// More info: http://www.dotnettips.info/post/2573
    /// </summary>
    public interface IMvcActionsDiscoveryService
    {
        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application.
        /// </summary>
        ICollection<MvcControllerViewModel> MvcControllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        ICollection<MvcControllerViewModel> GetAllSecuredControllerActionsWithPolicy(string policyName);
    }

    /// <summary>
    /// MvcActions Discovery Service Extensions
    /// </summary>
    public static class MvcActionsDiscoveryServiceExtensions
    {
        /// <summary>
        /// Adds IMvcActionsDiscoveryService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddMvcActionsDiscoveryService(this IServiceCollection services)
        {
            services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();
            return services;
        }
    }

    /// <summary>
    /// MvcActions Discovery Service
    /// </summary>
    public class MvcActionsDiscoveryService : IMvcActionsDiscoveryService
    {
        // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
        // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
        // threads but only one of the objects succeeds in creating a GetterInfo.
        private readonly ConcurrentDictionary<string, Lazy<ICollection<MvcControllerViewModel>>> _allSecuredActionsWithPloicy =
            new ConcurrentDictionary<string, Lazy<ICollection<MvcControllerViewModel>>>();

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
            MvcControllerViewModel currentController = null;

            var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (!(actionDescriptor is ControllerActionDescriptor descriptor))
                {
                    continue;
                }

                var controllerTypeInfo = descriptor.ControllerTypeInfo;
                var actionMethodInfo = descriptor.MethodInfo;

                if (lastControllerName != descriptor.ControllerName)
                {
                    currentController = new MvcControllerViewModel
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        ControllerAttributes = getAttributes(controllerTypeInfo),
                        ControllerDisplayName =
                           controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                           controllerTypeInfo.GetCustomAttribute<DisplayAttribute>()?.Name,
                        ControllerName = descriptor.ControllerName,
                    };
                    MvcControllers.Add(currentController);

                    lastControllerName = descriptor.ControllerName;
                }

                currentController?.MvcActions.Add(new MvcActionViewModel
                {
                    ControllerId = currentController.ControllerId,
                    ActionName = descriptor.ActionName,
                    ActionDisplayName =
                      actionMethodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                      actionMethodInfo.GetCustomAttribute<DisplayAttribute>()?.Name,
                    ActionAttributes = getAttributes(actionMethodInfo),
                    IsSecuredAction = isSecuredAction(controllerTypeInfo, actionMethodInfo)
                });
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
                        controller.MvcActions = controller.MvcActions.Where(
                            model => model.IsSecuredAction &&
                            (
                            model.ActionAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName ||
                            controller.ControllerAttributes.OfType<AuthorizeAttribute>().FirstOrDefault()?.Policy == policyName
                            )).ToList();
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
                                        return attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                                               attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;
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

    /// <summary>
    /// MvcAction ViewModel
    /// </summary>
    public class MvcActionViewModel
    {
        /// <summary>
        /// Returns the list of Attributes of the action method.
        /// </summary>
        public IList<Attribute> ActionAttributes { get; set; }

        /// <summary>
        /// Returns `DisplayNameAttribute` value of the action method.
        /// </summary>
        public string ActionDisplayName { get; set; }

        /// <summary>
        /// It's set to `{ControllerId}:{ActionName}`
        /// </summary>
        public string ActionId => $"{ControllerId}:{ActionName}";

        /// <summary>
        /// Return ControllerActionDescriptor.ActionName
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// It's set to `{AreaName}:{ControllerName}`
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// Returns true if the action method has an `AuthorizeAttribute`.
        /// </summary>
        public bool IsSecuredAction { get; set; }


        /// <summary>
        /// Returns `[{actionAttributes}]{ActionName}`
        /// </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var actionAttributes = string.Join(",", ActionAttributes.Select(a => a.GetType().Name.Replace(attribute, "")));
            return $"[{actionAttributes}]{ActionName}";
        }
    }

    /// <summary>
    /// MvcController ViewModel
    /// </summary>
    public class MvcControllerViewModel
    {
        /// <summary>
        /// Return `AreaAttribute.RouteValue`
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// Returns the list of the Controller's Attributes.
        /// </summary>
        public IList<Attribute> ControllerAttributes { get; set; }

        /// <summary>
        /// Returns the `DisplayNameAttribute` value
        /// </summary>
        public string ControllerDisplayName { get; set; }

        /// <summary>
        /// It's set to `{AreaName}:{ControllerName}`
        /// </summary>
        public string ControllerId => $"{AreaName}:{ControllerName}";

        /// <summary>
        /// Return ControllerActionDescriptor.ControllerName
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Returns the list of the Controller's action methods.
        /// </summary>
        public IList<MvcActionViewModel> MvcActions { get; set; } = new List<MvcActionViewModel>();

        /// <summary>
        /// Returns `[{controllerAttributes}]{AreaName}.{ControllerName}`
        /// </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var controllerAttributes = string.Join(",", ControllerAttributes.Select(a => a.GetType().Name.Replace(attribute, "")));
            return $"[{controllerAttributes}]{AreaName}.{ControllerName}";
        }
    }
}