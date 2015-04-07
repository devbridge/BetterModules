using System;
using System.Linq;
using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using NHibernate;

namespace BetterModules.Core
{
    /// <summary>
    /// Application Context Container
    /// </summary>
    public static class ApplicationContext
    {
        private static readonly object configurationLoaderLock = new object();

        private static volatile IConfiguration config;

        private static volatile bool configLoaded;

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        public static IConfiguration Config
        {
            get
            {
                if (!configLoaded)
                {
                    lock (configurationLoaderLock)
                    {
                        if (!configLoaded)
                        {
                            IConfigurationLoader configurationLoader = new DefaultConfigurationLoader();
                            config = configurationLoader.TryLoadConfig<DefaultConfigurationSection>();

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
        /// Creates the configured application dependencies container.
        /// </summary>
        /// <returns>The container builder.</returns>
        public static ContainerBuilder InitializeContainer(ContainerBuilder builder = null, IConfiguration configuration = null)
        {
            if (builder == null)
            {
                builder = new ContainerBuilder();
            }

            if (configuration != null)
            {
                Config = configuration;
            }

            builder.RegisterType<DefaultModulesRegistration>().As<IModulesRegistration>().SingleInstance();
            builder.RegisterType<DefaultSessionFactoryProvider>().As<ISessionFactoryProvider>().SingleInstance();
            builder.RegisterType<DefaultAssemblyLoader>().As<IAssemblyLoader>().SingleInstance();
            builder.RegisterType<DefaultUnitOfWorkFactory>().As<IUnitOfWorkFactory>().SingleInstance();
            builder.RegisterType<DefaultMappingResolver>().As<IMappingResolver>().SingleInstance();
            builder.RegisterType<DefaultWorkingDirectory>().As<IWorkingDirectory>().SingleInstance();
            builder.RegisterType<DefaultFetchingProvider>().As<IFetchingProvider>().SingleInstance();
            builder.RegisterType<DefaultUnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultRepository>().As<IRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultVersionChecker>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DefaultMigrationRunner>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DefaultPrincipalProvider>().As<IPrincipalProvider>().SingleInstance();
            builder.RegisterType<DefaultAssemblyManager>().As<IAssemblyManager>().SingleInstance();

            if (Config != null)
            {
                builder.RegisterInstance(Config).As<IConfiguration>().SingleInstance();
            }

            return builder;
        }

        /// <summary>
        /// Loads available assemblies.
        /// </summary>
        public static void LoadAssemblies()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                if (container == null)
                {
                    throw new CoreException("Application dependencies container is not initialized.");
                }

                IAssemblyManager assemblyManager = container.Resolve<IAssemblyManager>();

                // First add referenced modules...
                assemblyManager.AddReferencedModules();

                // ...then scan and register uploaded modules.
                assemblyManager.AddUploadedModules();

                var moduleRegistration = container.Resolve<IModulesRegistration>();
                moduleRegistration.InitializeModules();
            }
        }

        public static void RunDatabaseMigrations()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                if (container == null)
                {
                    throw new CoreException("Application dependencies container is not initialized.");
                }

                var migrationRunner = container.Resolve<IMigrationRunner>();
                var modulesRegistration = container.Resolve<IModulesRegistration>();

                var descriptors = modulesRegistration.GetModules().Select(m => m.ModuleDescriptor).ToList();
                migrationRunner.MigrateStructure(descriptors);
            }
        }
    }
}
