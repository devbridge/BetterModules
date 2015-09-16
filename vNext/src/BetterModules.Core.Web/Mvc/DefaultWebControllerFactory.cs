﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using BetterModules.Core.Web.Dependencies;
using BetterModules.Core.Web.Mvc.Routes;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Core.Web.Mvc
{
    /// <summary>
    /// Controller factory to work in web application context.
    /// </summary>
    public class DefaultWebControllerFactory : DefaultControllerFactory
    {
        private readonly PerWebRequestContainerProvider containerProvider;

        public DefaultWebControllerFactory(PerWebRequestContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// The controller instance.
        /// </returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller = null;

            if (controllerType != null && containerProvider.CurrentScope.IsRegistered(controllerType))
            {
                controller = containerProvider.CurrentScope.Resolve(controllerType) as IController;
            }

            return controller ?? base.GetControllerInstance(requestContext, controllerType);
        }

        /// <summary>
        /// Retrieves the controller type for the specified name and request context.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>
        /// The controller type.
        /// </returns>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var areaName = requestContext.RouteData.GetAreaName();

            if (string.IsNullOrEmpty(areaName))
            {
                return base.GetControllerType(requestContext, controllerName);
            }

            string key = (areaName + "-" + controllerName + "Controller").ToUpperInvariant();
            if (containerProvider.CurrentScope.IsRegisteredWithKey<IController>(key))
            {
                var controllerMetadata = containerProvider.CurrentScope.ResolveKeyed<Meta<IController>>(key);
                return controllerMetadata.Metadata["ControllerType"] as Type;
            }

            return base.GetControllerType(requestContext, controllerName);
        }
    }
}
