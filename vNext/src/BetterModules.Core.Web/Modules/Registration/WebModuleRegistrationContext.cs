using System;
using System.Collections.Generic;
using System.Linq;
using BetterModules.Core.Modules.Registration;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;

namespace BetterModules.Core.Web.Modules.Registration
{
    public class WebModuleRegistrationContext : ModuleRegistrationContext
    {
        public WebModuleRegistrationContext(WebModuleDescriptor moduleDescriptor) : base(moduleDescriptor)
        {
            Namespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            RouteBuilder = new RouteBuilder();
        }

        protected ICollection<string> Namespaces { get; private set; }

        internal IRouteBuilder RouteBuilder { get; private set; }

        public override string GetRegistrationName()
        {
            return ((WebModuleDescriptor)ModuleDescriptor).AreaName.ToLowerInvariant();
        }

        public IRouter MapRoute(string name, string url)
        {
            return MapRoute(name, url, null);
        }

        public IRouter MapRoute(string name, string url, string[] namespaces)
        {
            return MapRoute(name, url, null, namespaces);
        }


        public IRouter MapRoute(string name, string url, object defaults)
        {
            return MapRoute(name, url, defaults, null);
        }


        public IRouter MapRoute(string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(name, url, defaults, null, namespaces);
        }

        public IRouter MapRoute(string name, string url, object defaults, object constraints)
        {
            return MapRoute(name, url, defaults, constraints, null);
        }

        public IRouter MapRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if ((namespaces == null) && (this.Namespaces != null))
            {
                namespaces = Namespaces.ToArray<string>();
            }
            RouteBuilder.MapRoute(name, url, defaults, constraints, namespaces);
            var router = RouteBuilder.Build();
            route.DataTokens["area"] = ((WebModuleDescriptor)ModuleDescriptor).AreaName;
            bool flag = (namespaces == null) || (namespaces.Length == 0);
            route.DataTokens["UseNamespaceFallback"] = flag;
            return route;
        }

        public void IgnoreRoute(string url)
        {
            Routes.Ignore(url);
        }

        public void IgnoreRoute(string url, object constraints)
        {
            Routes.Ignore(url, constraints);
        }
    }
}
