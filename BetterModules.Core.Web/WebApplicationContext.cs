using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using BetterModules.Core.Web.Configuration;
using BetterModules.Core.Web.Dependencies;
using BetterModules.Core.Web.Environment.Assemblies;
using BetterModules.Core.Web.Environment.Host;
using BetterModules.Core.Web.Modules;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc;
using BetterModules.Core.Web.Mvc.Commands;
using BetterModules.Core.Web.Mvc.Extensions;
using BetterModules.Core.Web.Mvc.Routes;
using BetterModules.Core.Web.Security;
using BetterModules.Core.Web.Services.Caching;
using BetterModules.Core.Web.Web;
using BetterModules.Core.Web.Web.EmbeddedResources;
using RazorGenerator.Mvc;

namespace BetterModules.Core.Web
{
    /// <summary>
    /// Web Application Context Container
    /// </summary>
    public static class WebApplicationContext
    {
        private static readonly object configurationLoaderLock = new object();

        private static volatile IWebConfiguration config;
        
        private static volatile bool configLoaded;

        private static bool isTestMode;

        public static bool IsTestMode
        {
            get
            {
                return isTestMode;
            }
            set
            {
                isTestMode = value;
            }
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        public static IWebConfiguration Config
        {
            get
            {
                if (!configLoaded)
                {
                    lock (configurationLoaderLock)
                    {
                        if (!configLoaded)
                        {
                            IConfigurationLoader configurationLoader = new DefaultWebConfigurationLoader();
                            config = configurationLoader.TryLoadConfig<DefaultWebConfigurationSection>();

                            if (config == null)
                            {
                                config = new DefaultWebConfigurationSection();
                            }

                            configLoaded = true;
                        }
                    }
                    configLoaded = true;
                }

                return config;
            }
            set
            {
                lock (configurationLoaderLock)
                {
                    configLoaded = true;
                    config = value;
                }
            }
        }

        /// <summary>
        /// Constructs the host context.
        /// </summary>
        /// <returns>Constructed host context.</returns>
        public static IWebApplicationHost RegisterHost()
        {
            IWebApplicationHost host;
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                if (container == null)
                {
                    throw new CoreException("Web application dependencies container is not initialized.");
                }

                host = container.Resolve<IWebApplicationHost>();
                if (host == null)
                {
                    throw new CoreException("Web application host context was not created.");
                }
            }

            return host;
        }

        /// <summary>
        /// Creates the configured web application dependencies container.
        /// </summary>
        /// <returns>The container builder.</returns>
        public static ContainerBuilder InitializeContainer(ContainerBuilder builder = null, IWebConfiguration configuration = null)
        {
            if (builder == null)
            {
                builder = new ContainerBuilder();
            }

            if (configuration != null)
            {
                Config = configuration;
            }

            builder = ApplicationContext.InitializeContainer(builder, Config);

            builder.RegisterType<DefaultWebModulesRegistration>()
                .As<IModulesRegistration>()
                .As<IWebModulesRegistration>()
                .SingleInstance();

            builder.RegisterType<DefaultWebControllerFactory>().SingleInstance();
            builder.RegisterType<DefaultEmbeddedResourcesProvider>().As<IEmbeddedResourcesProvider>().SingleInstance();
            builder.RegisterType<DefaultHttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<DefaultControllerExtensions>().As<IControllerExtensions>().SingleInstance();
            builder.RegisterType<DefaultCommandResolver>().As<ICommandResolver>().InstancePerLifetimeScope();
            builder.RegisterInstance(new DefaultRouteTable(RouteTable.Routes)).As<IRouteTable>().SingleInstance();
            builder.RegisterType<PerWebRequestContainerProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultWebPrincipalProvider>().As<IPrincipalProvider>().SingleInstance();
            builder.RegisterType<DefaultWebAssemblyManager>().As<IAssemblyManager>().SingleInstance();
            builder.RegisterType<HttpRuntimeCacheService>().As<ICacheService>().SingleInstance();
            builder.RegisterType<DefaultWebApplicationHost>().As<IWebApplicationHost>().SingleInstance();

            if (Config != null)
            {
                builder.RegisterInstance(Config)
                    .As<IConfiguration>()
                    .As<IWebConfiguration>()
                    .SingleInstance();
            }

            return builder;
        }

        /// <summary>
        /// Loads available assemblies.
        /// </summary>
        public static void LoadAssemblies()
        {
            ApplicationContext.LoadAssemblies();

            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                if (HostingEnvironment.IsHosted)
                {
                    HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourcesVirtualPathProvider(container.Resolve<IEmbeddedResourcesProvider>()));
                }
                else
                {
                    if (!IsTestMode)
                    {
                        throw new CoreException("Failed to register EmbeddedResourcesVirtualPathProvider as a virtual path provider.");
                    }
                }

                ControllerBuilder.Current.SetControllerFactory(container.Resolve<DefaultWebControllerFactory>());

                // Register precompiled views for all the assemblies
                var precompiledAssemblies = new List<PrecompiledViewAssembly>();

                var moduleRegistration = container.Resolve<IModulesRegistration>();
                moduleRegistration.GetModules().Select(m => m.ModuleDescriptor).Distinct().ToList().ForEach(
                    descriptor =>
                    {
                        var webDescriptor = descriptor as WebModuleDescriptor;
                        if (webDescriptor != null)
                        {
                            var precompiledAssembly = new PrecompiledViewAssembly(descriptor.GetType().Assembly, string.Format("~/Areas/{0}/", webDescriptor.AreaName))
                            {
                                UsePhysicalViewsIfNewer = false
                            };
                            precompiledAssemblies.Add(precompiledAssembly);
                        }
                    });

                var engine = new CompositePrecompiledMvcEngine(precompiledAssemblies.ToArray());
                ViewEngines.Engines.Add(engine);
                VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
            }
        }
    }
}
