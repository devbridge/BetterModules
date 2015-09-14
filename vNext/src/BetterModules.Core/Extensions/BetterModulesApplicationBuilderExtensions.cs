using System.Linq;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Modules.Registration;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Extensions
{
    public static class BetterModulesApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBetterModules(this IApplicationBuilder builder)
        {
            RunDatabaseMigrations(builder);

            return builder;
        }

        private static void RunDatabaseMigrations(IApplicationBuilder builder)
        {
            var migrationRunner = builder.ApplicationServices.GetService<IMigrationRunner>();
            var modulesRegistration = builder.ApplicationServices.GetService<IModulesRegistration>();

            var descriptors = modulesRegistration.GetModules().Select(m => m.ModuleDescriptor).ToList();
            migrationRunner.MigrateStructure(descriptors);
        }
    }
}