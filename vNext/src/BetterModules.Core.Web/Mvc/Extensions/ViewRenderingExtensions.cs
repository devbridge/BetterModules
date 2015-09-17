using System.IO;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

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
            var services = controller.ActionContext.HttpContext.RequestServices;
            var compositeViewEngine = services.GetRequiredService<ICompositeViewEngine>();
            var htmlHelperOptions = services.GetRequiredService<IOptions<MvcViewOptions>>().Options.HtmlHelperOptions;

            if (string.IsNullOrEmpty(viewName) || viewName.ToLower() == controller.ActionContext.ActionDescriptor.Name.ToLower())
            {
                var areaName = controller.ActionContext.RouteData.Values["area"];
                var controllerName = controller.ActionContext.RouteData.Values["controller"];
                var actionName = controller.ActionContext.ActionDescriptor.Name;

                viewName = $"~/Areas/{areaName}/Views/{controllerName}/{actionName}.cshtml";
            }

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = compositeViewEngine.FindPartialView(controller.ActionContext, viewName);
                var viewContext = new ViewContext(controller.ActionContext, viewResult.View, controller.ViewData, controller.TempData, sw, htmlHelperOptions);
                if (enableFormContext && viewContext.FormContext == null)
                {
                    viewContext.FormContext = new FormContext();
                }

                viewResult.View.RenderAsync(viewContext);
                //viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
