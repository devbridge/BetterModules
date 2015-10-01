using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules.Registration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.Web.Modules.Registration
{
    /// <summary>
    /// Default modules registration implementation.
    /// </summary>
    public class DefaultWebModulesRegistration : DefaultModulesRegistration, IWebModulesRegistration
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebModulesRegistration" /> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultWebModulesRegistration(IAssemblyLoader assemblyLoader, ILoggerFactory loggerFactory)
            : base(assemblyLoader, loggerFactory) { }

        /// <summary>
        /// Finds the module by area name.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>Known module instance.</returns>
        public WebModuleDescriptor FindModuleByAreaName(string areaName)
        {
            ModuleRegistrationContext module;
            if (knownModules.TryGetValue(areaName.ToLowerInvariant(), out module))
            {
                return module.ModuleDescriptor as WebModuleDescriptor;
            }

            return null;
        }

        /// <summary>
        /// Determines whether module is registered by area name.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>
        ///   <c>true</c> if module by area name is registered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsModuleRegisteredByAreaName(string areaName)
        {
            return knownModules.ContainsKey(areaName.ToLowerInvariant());
        }

        /// <summary>
        /// Registers the types.
        /// </summary>
        /// <param name="registrationContext">The registration context.</param>
        /// <param name="services"></param>
        protected override void RegisterModuleDescriptor(ModuleRegistrationContext registrationContext, IServiceCollection services)
        {
            var webContext = registrationContext as WebModuleRegistrationContext;
            if (webContext != null)
            {
                var webDescriptor = (WebModuleDescriptor)webContext.ModuleDescriptor;
                webDescriptor.RegisterModuleCommands(webContext, services);
            }

            base.RegisterModuleDescriptor(registrationContext, services);
        }
    }
}
