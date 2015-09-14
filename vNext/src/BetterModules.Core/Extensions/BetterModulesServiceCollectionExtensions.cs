using System;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using IConfiguration = Microsoft.Framework.Configuration.IConfiguration;

namespace BetterModules.Core.Extensions
{
    public static class BetterModulesServiceCollectionExtensions
    {
        public static IServiceCollection AddBetterModules(this IServiceCollection services, IConfiguration configuration)
        {
            LoadConfiguration(services, configuration);
            ConfigureDefaultServices(services);

            LoadAssemblies(services);

            return services;
        }

        public static IServiceCollection AddBetterModules(this IServiceCollection services, IConfiguration configuration, Action<ILoggerFactory> configureLoggers)
        {
            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            configureLoggers(loggerFactory);

            return services.AddBetterModules(configuration);
        }

        private static void ConfigureDefaultServices(IServiceCollection services)
        {
            services.AddSingleton<IModulesRegistration, DefaultModulesRegistration>();
            services.AddSingleton<ISessionFactoryProvider, DefaultSessionFactoryProvider>();
            services.AddSingleton<IAssemblyLoader, DefaultAssemblyLoader>();
            services.AddSingleton<IUnitOfWorkFactory, DefaultUnitOfWorkFactory>();
            services.AddSingleton<IMappingResolver, DefaultMappingResolver>();
            services.AddSingleton<IWorkingDirectory, DefaultWorkingDirectory>();
            services.AddSingleton<IFetchingProvider, DefaultFetchingProvider>();
            services.AddSingleton<IUnitOfWork, DefaultUnitOfWork>();
            services.AddSingleton<IRepository, DefaultRepository>();
            services.AddSingleton<IPrincipalProvider, DefaultPrincipalProvider>();
            services.AddSingleton<IAssemblyManager, DefaultAssemblyManager>();
            services.AddSingleton<IVersionChecker, DefaultVersionChecker>();
            services.AddSingleton<IMigrationRunner, DefaultMigrationRunner>();
        }

        private static void LoadConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultConfigurationSection>(configuration.GetSection("modulesSettings"));
            var provider = services.BuildServiceProvider();
            var config = provider.GetService<IOptions<DefaultConfigurationSection>>().Options;
            config.Database.ConnectionString = configuration[config.Database.ConnectionStringName];
            services.AddInstance<Configuration.IConfiguration>(config);
        }

        private static void LoadAssemblies(IServiceCollection services)
        {

            var provider = services.BuildServiceProvider();

            var assemblyManager = provider.GetService<IAssemblyManager>();

            // First add referenced modules...
            assemblyManager.AddReferencedModules();

            // ...then scan and register uploaded modules.
            assemblyManager.AddUploadedModules();

            var moduleRegistration = provider.GetService<IModulesRegistration>();

            moduleRegistration.InitializeModules(services);

            services.AddInstance(moduleRegistration);

            ModulesRegistrationSingleton.Instance = moduleRegistration;
        }
    }
}