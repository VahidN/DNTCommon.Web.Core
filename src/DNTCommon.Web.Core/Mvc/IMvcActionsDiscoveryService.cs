using System.Collections.Generic;

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
}