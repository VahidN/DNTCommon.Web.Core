using System.Collections.Generic;
using System;
using System.Linq;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// MvcController ViewModel
    /// </summary>
    public class MvcControllerViewModel
    {
        /// <summary>
        /// Return `AreaAttribute.RouteValue`
        /// </summary>
        public string? AreaName { get; set; }

        /// <summary>
        /// Returns the list of the Controller's Attributes.
        /// </summary>
        public IList<Attribute> ControllerAttributes { get; } = new List<Attribute>();

        /// <summary>
        /// Returns the `DisplayNameAttribute` value
        /// </summary>
        public string? ControllerDisplayName { get; set; }

        /// <summary>
        /// It's set to `{AreaName}:{ControllerName}`
        /// </summary>
        public string ControllerId => $"{AreaName}:{ControllerName}";

        /// <summary>
        /// Return ControllerActionDescriptor.ControllerName
        /// </summary>
        public string ControllerName { get; set; } = default!;

        /// <summary>
        /// Returns the list of the Controller's action methods.
        /// </summary>
        public IList<MvcActionViewModel> MvcActions { get; } = new List<MvcActionViewModel>();

        /// <summary>
        /// Returns `[{controllerAttributes}]{AreaName}.{ControllerName}`
        /// </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var controllerAttributes = string.Join(",",
                ControllerAttributes.Select(a => a.GetType().Name.Replace(attribute, "", StringComparison.Ordinal)));
            return $"[{controllerAttributes}]{AreaName}.{ControllerName}";
        }
    }
}