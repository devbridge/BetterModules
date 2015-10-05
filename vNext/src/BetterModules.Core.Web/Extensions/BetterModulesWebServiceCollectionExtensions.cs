using System;
using BetterModules.Core.Extensions;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using BetterModules.Core.Web.Configuration;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc.Extensions;
using BetterModules.Core.Web.Security;
using BetterModules.Core.Web.Web;
using BetterModules.Core.Web.Web.EmbeddedResources;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.Web.Extensions
{
    public static class BetterModulesWebServiceCollectionExtensions
    {
        public static IServiceCollection AddBetterModulesWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadWebConfiguration(configuration);
            services.ConfigureDefaultWebServices();
            services.LoadWebAssemblies();
            BetterModulesCoreServiceCollectionExtensions.RunDatabaseMigrations(services);

            return services;
        }

        public static IServiceCollection AddBetterModulesWeb(this IServiceCollection services, IConfiguration configuration, Action<ILoggerFactory> configureLoggers)
        {
            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            configureLoggers(loggerFactory);

            return services.AddBetterModulesWeb(configuration);
        }

        private static void ConfigureDefaultWebServices(this IServiceCollection services)
        {
            services.ConfigureDefaultServices();

            services.AddSingleton<IModulesRegistration, DefaultWebModulesRegistration>();
            services.AddSingleton<IWebModulesRegistration, DefaultWebModulesRegistration>();

            services.AddSingleton<IHttpContextAccessor, DefaultHttpContextAccessor>();
            services.AddSingleton<IControllerExtensions, DefaultControllerExtensions>();
            services.AddSingleton<IPrincipalProvider, DefaultWebPrincipalProvider>();
        }

        private static void LoadWebConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadConfiguration(configuration);
            services.Configure<DefaultWebConfigurationSection>(configuration);
            services.Configure<DefaultWebConfigurationSection>(opt =>
            {
                opt.Database.ConnectionString = configuration[opt.Database.ConnectionStringName];
            });
        }

        private static void LoadWebAssemblies(this IServiceCollection services)
        {
            services.LoadAssemblies();

            var provider = services.BuildServiceProvider();

            var modulesRegistration = provider.GetService<IModulesRegistration>();
            var resourceProvider = ActivatorUtilities.CreateInstance<DefaultEmbeddedResourceProvider>(provider);
            resourceProvider.LoadEmbeddedResourcesFrom(modulesRegistration.GetModules());
            services.AddInstance<IEmbeddedResourceProvider>(resourceProvider);
            services.Configure<RazorViewEngineOptions>(opt =>
            {
                opt.FileProvider = resourceProvider;
            });
        }
    }
}