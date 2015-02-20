using System;
using System.Web.Mvc;
using System.Web.Routing;
using BetterModules.Core.Web;
using BetterModules.Core.Web.Environment.Host;

namespace BetterModules.Mvc5.Sandbox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWebApplicationHost modularAppHost;

        protected void Application_Start()
        {
            modularAppHost = WebApplicationContext.RegisterHost();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            modularAppHost.OnApplicationStart(this);
        }

        protected void Application_BeginRequest()
        {
            modularAppHost.OnBeginRequest(this);
        }

        protected void Application_EndRequest()
        {
            modularAppHost.OnEndRequest(this);
        }

        protected void Application_Error()
        {
            modularAppHost.OnApplicationError(this);
        }

        protected void Application_End()
        {
            modularAppHost.OnApplicationEnd(this);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            modularAppHost.OnAuthenticateRequest(this);
        }
    }
}
