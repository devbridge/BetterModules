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
        public WebModuleRegistrationContext(WebModuleDescriptor moduleDescriptor) : base(moduleDescriptor) {}

        public override string GetRegistrationName()
        {
            return ((WebModuleDescriptor)ModuleDescriptor).AreaName.ToLowerInvariant();
        }
    }
}
