using System;
using System.Linq;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Infrastructure.Commands;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.Extensions
{
    public static class BetterModulesCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddBetterModulesCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.LoadConfiguration(configuration);
            services.ConfigureDefaultServices();

            services.LoadAssemblies();

            RunDatabaseMigrations(services);

            return services;
        }

        public static IServiceCollection AddBetterModulesCore(this IServiceCollection services, IConfiguration configuration, Action<ILoggerFactory> configureLoggers)
        {
            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            configureLoggers(loggerFactory);

            return services.AddBetterModulesCore(configuration);
        }

        public static void ConfigureDefaultServices(this IServiceCollection services)
        {
            services.AddSingleton<IModulesRegistration, DefaultModulesRegistration>();
            services.AddSingleton<ISessionFactoryProvider, DefaultSessionFactoryProvider>();
            services.AddSingleton<IAssemblyLoader, DefaultAssemblyLoader>();
            services.AddSingleton<IUnitOfWorkFactory, DefaultUnitOfWorkFactory>();
            services.AddSingleton<IMappingResolver, DefaultMappingResolver>();
            services.AddSingleton<IWorkingDirectory, DefaultWorkingDirectory>();
            services.AddSingleton<IPrincipalProvider, DefaultPrincipalProvider>();
            services.AddSingleton<IAssemblyManager, DefaultAssemblyManager>();
            services.AddSingleton<IVersionChecker, DefaultVersionChecker>();
            services.AddSingleton<IMigrationRunner, DefaultMigrationRunner>();

            services.AddScoped<ICommandResolver, DefaultCommandResolver>();
            services.AddScoped<IUnitOfWork, DefaultUnitOfWork>();
            services.AddScoped<IRepository, DefaultRepository>();
        }

        public static void LoadConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultConfigurationSection>(configuration);
        }

        public static void LoadAssemblies(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var assemblyManager = provider.GetService<IAssemblyManager>();

            // First add referenced modules...
            assemblyManager.AddReferencedModules();

            // ...then scan and register uploaded modules.
            assemblyManager.AddUploadedModules();

            var moduleRegistration = ActivatorUtilities.GetServiceOrCreateInstance<IModulesRegistration>(provider);
            moduleRegistration.InitializeModules(services);
            services.AddInstance(moduleRegistration);

            ModulesRegistrationSingleton.Instance = moduleRegistration;
        }

        public static void RunDatabaseMigrations(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var migrationRunner = provider.GetService<IMigrationRunner>();
            var modulesRegistration = provider.GetService<IModulesRegistration>();

            var descriptors = modulesRegistration.GetModules().Select(x => x.ModuleDescriptor).ToList();
            migrationRunner.MigrateStructure(descriptors);
        }
    }
}