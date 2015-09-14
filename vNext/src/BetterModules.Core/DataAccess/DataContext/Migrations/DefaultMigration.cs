using System;
using System.Linq;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Models;
using BetterModules.Core.Modules.Registration;
using FluentMigrator;
using FluentMigrator.Builders.IfDatabase;

namespace BetterModules.Core.DataAccess.DataContext.Migrations
{
    public abstract class DefaultMigration : Migration
    {
        protected const string PostgresThrowNotSupportedErrorSql =
            "RAISE EXCEPTION 'NOT SUPPORTED IN CURRENT VERSION!';";

        protected const string OracleThrowNotSupportedErrorSql =
            "raise_application_error(-1, 'NOT SUPPORTED IN CURRENT VERSION!');";

        private readonly string moduleName;

        private string schemaName;

        public string SchemaName
        {
            get { return schemaName ?? (schemaName = SchemaNameProvider.GetSchemaName(moduleName)); }
        }

        public DefaultMigration(string moduleName)
        {
            this.moduleName = moduleName;
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(
                        module => module.ModuleDescriptor != null && module.ModuleDescriptor.Name == moduleName);
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
        }

        public DefaultMigration(Type moduleDescriptorType)
        {
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(
                        module => module.ModuleDescriptor != null && module.ModuleDescriptor.GetType() == moduleDescriptorType);
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
        }
        
        public DefaultMigration()
        {
            var assembly = this.GetType().Assembly;
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(
                        module => module.ModuleDescriptor?.AssemblyName == assembly.GetName());
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            throw new CoreException("Down migration not possible.",
                new NotSupportedException("Application doesn't support DOWN migrations."));
        }

        protected IIfDatabaseExpressionRoot IfSqlServer()
        {
            return IfDatabase("SqlServer");
        }

        protected IIfDatabaseExpressionRoot IfPostgres()
        {
            return IfDatabase("Postgres");
        }

        protected IIfDatabaseExpressionRoot IfOracle()
        {
            return IfDatabase("Oracle");
        }
    }
}