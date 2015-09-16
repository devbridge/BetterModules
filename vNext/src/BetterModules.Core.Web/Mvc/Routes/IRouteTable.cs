using System.Web.Routing;

namespace BetterModules.Core.Web.Mvc.Routes
{
    /// <summary>
    /// Defines the contract that a class must contain routes collection.
    /// </summary>
    public interface IRouteTable
    {
        /// <summary>
        /// Gets the route collection.
        /// </summary>
        /// <value>
        /// The route collection.
        /// </value>
        RouteCollection Routes { get; }
    }
}
