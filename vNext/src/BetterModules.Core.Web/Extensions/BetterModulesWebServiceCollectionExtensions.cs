using System.Collections.Generic;
using System.Linq;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Extensions;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using BetterModules.Core.Web.Configuration;
using BetterModules.Core.Web.Environment.Assemblies;
using BetterModules.Core.Web.Modules;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc;
using BetterModules.Core.Web.Mvc.Commands;
using BetterModules.Core.Web.Mvc.Extensions;
using BetterModules.Core.Web.Security;
using BetterModules.Core.Web.Web;
using BetterModules.Core.Web.Web.EmbeddedResources;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace BetterModules.Core.Web.Extensions
{
    public static class BetterModulesWebServiceCollectionExtensions
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

            services.AddSingleton<IEmbeddedResourcesProvider, DefaultEmbeddedResourcesProvider>();
            services.AddSingleton<IHttpContextAccessor, DefaultHttpContextAccessor>();
            services.AddSingleton<IControllerExtensions, DefaultControllerExtensions>();
            services.AddSingleton<ICommandResolver, DefaultCommandResolver>();
            services.AddSingleton<IPrincipalProvider, DefaultWebPrincipalProvider>();
            services.AddSingleton<IAssemblyManager, DefaultWebAssemblyManager>();
        }

        private static void LoadWebConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultWebConfigurationSection>(configuration);
            var provider = services.BuildServiceProvider();
            var config = provider.GetService<IOptions<DefaultWebConfigurationSection>>().Options;
            if (config?.Database != null)
            {
                config.Database.ConnectionString = configuration[config.Database.ConnectionStringName];
            }
            services.AddInstance<IWebConfiguration>(config);
            services.AddInstance<Core.Configuration.IConfiguration>(config);
        }

        private static void LoadWebAssemblies(this IServiceCollection services)
        {
            services.LoadAssemblies();

            //TODO: find out how to register EmbeddedResourcesVirtualPathProvider
            //if (HostingEnvironment.IsHosted)
            //{
            //    HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourcesVirtualPathProvider(container.Resolve<IEmbeddedResourcesProvider>()));
            //}
            //else
            //{
            //    if (!IsTestMode)
            //    {
            //        throw new CoreException("Failed to register EmbeddedResourcesVirtualPathProvider as a virtual path provider.");
            //    }
            //}

            //TODO: find out how to add precompiled views for assemblies
            // Register precompiled views for all the assemblies
            //var precompiledAssemblies = new List<PrecompiledViewAssembly>();

            //var provider = services.BuildServiceProvider();
            //var moduleRegistration = provider.GetService<IModulesRegistration>();
            //moduleRegistration.GetModules().Select(m => m.ModuleDescriptor).Distinct().ToList().ForEach(
            //    descriptor =>
            //    {
            //        var webDescriptor = descriptor as WebModuleDescriptor;
            //        if (webDescriptor != null)
            //        {
            //            var precompiledAssembly = new PrecompiledViewAssembly(descriptor.GetType().Assembly,
            //                $"~/Areas/{webDescriptor.AreaName}/")
            //            {
            //                UsePhysicalViewsIfNewer = false
            //            };
            //            precompiledAssemblies.Add(precompiledAssembly);
            //        }
            //    });

            //var engine = new CompositePrecompiledMvcEngine(precompiledAssemblies.ToArray());
            //services.Configure<MvcViewOptions>(options =>
            //{
            //    options.ViewEngines.Add(engine);
            //});
        }
    }
}