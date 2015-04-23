using Autofac;
using BetterModules.Core.Dependencies;

namespace BetterModules.Core.Modules.Registration
{
    internal static class ModulesRegistrationSingleton
    {
        private static IModulesRegistration modulesRegistration;

        public static IModulesRegistration Instance
        {
            get
            {
                if (modulesRegistration == null)
                {
                    using (var lifetimeScope = ContextScopeProvider.CreateChildContainer())
                    {
                        modulesRegistration = lifetimeScope.Resolve<IModulesRegistration>();
                    }
                }
                return modulesRegistration;
            }
        }
    }
}
