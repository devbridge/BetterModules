using System;
using System.Linq;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Modules.Registration;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Web.Extensions
{
    public static class BetterModulesApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBetterModules(this IApplicationBuilder app, IHostingEnvironment env)
        {
            RunDatabaseMigrations(app.ApplicationServices);
            if (env.IsDevelopment())
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.Query["restart"] == "1")
                    {
                        //find a way to restart a server
                        return;
                    }
                    await next.Invoke();
                });
            }
            return app;
        }

        private static void RunDatabaseMigrations(IServiceProvider provider)
        {
            var migrationRunner = provider.GetService<IMigrationRunner>();
            var modulesRegistration = provider.GetService<IModulesRegistration>();

            var descriptors = modulesRegistration.GetModules().Select(x => x.ModuleDescriptor).ToList();
            migrationRunner.MigrateStructure(descriptors);
        }
    }
}
