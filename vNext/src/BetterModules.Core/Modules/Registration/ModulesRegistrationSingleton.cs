using System;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Internal;

namespace BetterModules.Core.Modules.Registration
{
    public static class ModulesRegistrationSingleton
    {
        public static IModulesRegistration Instance { get; set; }
    }
}