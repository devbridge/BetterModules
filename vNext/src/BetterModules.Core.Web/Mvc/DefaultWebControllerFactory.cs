using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Core.Web.Mvc
{
    // Custom ControllerFactory is no longer needed

    ///// <summary>
    ///// Controller factory to work in web application context.
    ///// </summary>
    //public class DefaultWebControllerFactory : DefaultControllerFactory
    //{
    //    public DefaultWebControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators) 
    //        : base(controllerActivator, propertyActivators)
    //    {
    //    }

    //    /// <summary>
    //    /// Retrieves the controller instance for the specified request context and controller type.
    //    /// </summary>
    //    /// <param name="actionContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
    //    /// <param name="controllerType">The type of the controller.</param>
    //    /// <returns>
    //    /// The controller instance.
    //    /// </returns>
    //    protected Controller GetControllerInstance(ActionContext actionContext, Type controllerType)
    //    {
    //        var provider = actionContext.HttpContext.RequestServices;
    //        Controller controller = null;

    //        if (controllerType != null)
    //        {
    //            controller = provider.GetService(controllerType) as Controller;
    //        }

    //        return controller ?? (Controller)CreateController(actionContext);
    //    }

    //    /// <summary>
    //    /// Retrieves the controller type for the specified action context.
    //    /// </summary>
    //    /// <param name="actionContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
    //    /// <returns>
    //    /// The controller type.
    //    /// </returns>
    //    protected Type GetControllerType(ActionContext actionContext)
    //    {
    //        var descriptor = (ControllerActionDescriptor)actionContext.ActionDescriptor;

    //        return descriptor.ControllerTypeInfo.AsType();
    //    }
    //}
}
