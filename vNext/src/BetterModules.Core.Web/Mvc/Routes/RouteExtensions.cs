﻿using System.Web.Routing;
using Microsoft.Web.Mvc;

namespace BetterModules.Core.Web.Mvc.Routes
{
    /// <summary>
    /// Route extensions container.
    /// </summary>
    public static class RouteExtensions
    {
        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns>Area name.</returns>
        public static string GetAreaName(this RouteData routeData)
        {
            return AreaHelpers.GetAreaName(routeData);
        }
    }
}

