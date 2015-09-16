﻿using System.IO;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Core.Web.Mvc.Extensions
{
    public static class ViewRenderingExtensions
    {
        /// <summary>
        /// Renders the view to string.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <param name="enableFormContext">if set to <c>true</c> enable form context.</param>
        /// <returns>View, rendered to string</returns>
        public static string RenderViewToString(this CoreControllerBase controller, string viewName, object model, bool enableFormContext = false)
        {
            if (string.IsNullOrEmpty(viewName) || viewName.ToLower() == controller.ControllerContext.RouteData.GetRequiredString("action").ToLower())
            {
                var areaName = controller.ControllerContext.RouteData.GetRequiredString("area");
                var controllerName = controller.ControllerContext.RouteData.GetRequiredString("controller");
                var actionName = controller.ControllerContext.RouteData.GetRequiredString("action");

                viewName = string.Format("~/Areas/{0}/Views/{1}/{2}.cshtml", areaName, controllerName, actionName);
            }

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                if (enableFormContext && viewContext.FormContext == null)
                {
                    viewContext.FormContext = new FormContext();
                }

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
