using BetterModules.Core.Extensions;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using BetterModules.Core.Web.Configuration;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc;
using BetterModules.Core.Web.Mvc.Extensions;
using BetterModules.Core.Web.Security;
using BetterModules.Core.Web.Web;
using BetterModules.Core.Web.Web.EmbeddedResources;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Web.Extensions
{
    public static class BetterModulesServiceCollectionExtensions
    {
        public static IServiceCollection AddBetterModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadWebConfiguration(configuration);
            services.ConfigureDefaultWebServices();
            services.LoadWebAssemblies();
            

            return services;
        }

        private static void ConfigureDefaultWebServices(this IServiceCollection services)
        {
            services.ConfigureDefaultServices();

            services.AddSingleton<IModulesRegistration, DefaultWebModulesRegistration>();
            services.AddSingleton<IWebModulesRegistration, DefaultWebModulesRegistration>();

            services.AddSingleton<IControllerFactory, DefaultWebControllerFactory>();

            services.AddSingleton<IHttpContextAccessor, DefaultHttpContextAccessor>();
            services.AddSingleton<IControllerExtensions, DefaultControllerExtensions>();
            services.AddSingleton<IPrincipalProvider, DefaultWebPrincipalProvider>();
        }

        private static void LoadWebConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultWebConfigurationSection>(configuration);
        }

        private static void LoadWebAssemblies(this IServiceCollection services)
        {
            services.LoadAssemblies();

            var provider = services.BuildServiceProvider();

            var modulesRegistration = provider.GetService<IModulesRegistration>();
            var resourceProvider = ActivatorUtilities.CreateInstance<DefaultEmbeddedResourceProvider>(provider);
            resourceProvider.LoadEmbeddedResourcesFrom(modulesRegistration.GetModules());
            services.AddInstance<IEmbeddedResourceProvider>(resourceProvider);
        }
    }
}