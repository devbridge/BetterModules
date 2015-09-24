﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BetterModules.Core.Configuration;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.Processors.Postgres;
using FluentMigrator.Runner.Processors.SqlServer;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace BetterModules.Core.DataAccess.DataContext.Migrations
{    
    /// <summary>
    /// Default database migrations runner.
    /// </summary>
    public class DefaultMigrationRunner : IMigrationRunner
    {
        /// <summary>
        /// Database type to run migrations.
        /// </summary>
        private const DatabaseType databaseType = DatabaseType.SqlAzure;

        /// <summary>
        /// Timeout for one migration execution.
        /// </summary>
        private static readonly TimeSpan migrationTimeout = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Current class logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Provides assembly and types loading methods.
        /// </summary>
        private readonly IAssemblyLoader assemblyLoader;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly DefaultConfigurationSection configuration;

        /// <summary>
        /// The version checker
        /// </summary>
        private readonly IVersionChecker versionChecker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationRunner" /> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        /// <param name="configurationAccessor">The configuration accessor.</param>
        /// <param name="versionChecker">The version checker.</param>
        public DefaultMigrationRunner(IAssemblyLoader assemblyLoader,
            IOptions<DefaultConfigurationSection> configuration, 
            IVersionChecker versionChecker,
            ILoggerFactory loggerFactory)
        {
            this.assemblyLoader = assemblyLoader;
            this.configuration = configuration.Options;
            this.versionChecker = versionChecker;
            logger = loggerFactory.CreateLogger(typeof (DefaultMigrationRunner).FullName);
        }

        /// <summary>
        /// Runs migrations from the specified assemblies.
        /// </summary>
        public void MigrateStructure(IList<ModuleDescriptor> moduleDescriptors)
        {
            var versions = new Dictionary<long, IList<ModuleDescriptor>>();
            var moduleWithMigrations = new Dictionary<ModuleDescriptor, IList<Type>>();

            foreach (var moduleDescriptor in moduleDescriptors)
            {
                var migrationTypes = assemblyLoader.GetLoadableTypes(moduleDescriptor.GetType().Assembly, typeof(Migration));
                if (migrationTypes != null)
                {
                    var types = migrationTypes as IList<Type> ?? migrationTypes.ToList();
                    moduleWithMigrations.Add(moduleDescriptor, types);

                    foreach (var migrationType in types)
                    {
                        var migrationAttributes = migrationType.GetCustomAttributes(typeof(MigrationAttribute), true);
                        if (migrationAttributes.Length > 0)
                        {
                            var attribute = migrationAttributes[0] as MigrationAttribute;
                            if (attribute != null)
                            {
                                if (!versions.ContainsKey(attribute.Version))
                                {
                                    versions[attribute.Version] = new List<ModuleDescriptor>();
                                }
                                versions[attribute.Version].Add(moduleDescriptor);
                            }
                        }
                    }
                }
            }

            foreach (var version in versions.OrderBy(f => f.Key))
            {
                var versionNumber = version.Key;

                foreach (var moduleDescriptor in version.Value)
                {
                    if (!versionChecker.VersionExists(moduleDescriptor.Name, versionNumber))
                    {
                        var migrationTypes = moduleWithMigrations[moduleDescriptor];
                        Migrate(moduleDescriptor, migrationTypes, versionNumber);

                        versionChecker.AddVersion(moduleDescriptor.Name, versionNumber);
                    }
                }
            }
        }

        /// <summary>
        /// Runs database migrations of the specified module descriptor.
        /// </summary>
        /// <param name="moduleDescriptor">The module descriptor.</param>        
        /// <param name="up">if set to <c>true</c> migrates up; otherwise migrates down.</param>
        private void Migrate(ModuleDescriptor moduleDescriptor, IEnumerable<Type> migrationTypes = null, long? version = null)
        {
            var announcer = new TextWriterAnnouncer(
                s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            logger.LogInformation(string.Concat("Migration on ", moduleDescriptor.Name, ". ", s));
                        }
                    });

            var assembly = moduleDescriptor.GetType().Assembly;

            if (migrationTypes == null)
            {
                migrationTypes = assemblyLoader.GetLoadableTypes(assembly, typeof(Migration));
            }

            if (migrationTypes == null || !migrationTypes.Any())
            {
                logger.LogInformation(string.Concat("Migration on ", moduleDescriptor.Name, ". No migrations found."));
                return;
            }

            var migrationContext = new RunnerContext(announcer)
            {
                Namespace = migrationTypes.First().Namespace
            };            

            IMigrationProcessorOptions options = new ProcessorOptions
                {
                    PreviewOnly = false,
                    Timeout = (int)migrationTimeout.TotalSeconds
                };
           
            IMigrationProcessor processor;
            IDbConnection dbConnection = null;

            string connectionString;
            if (!string.IsNullOrEmpty(configuration.Database.ConnectionString))
            {
                connectionString = configuration.Database.ConnectionString;
            }
            //else if (!string.IsNullOrEmpty(configuration.ConnectionStringName))
            //{
            //    connectionString = ConfigurationManager.ConnectionStrings[configuration.ConnectionStringName].ConnectionString;
            //}
            else
            {
                throw new System.Configuration.ConfigurationException("Missing connection string.");
            }

            if (databaseType == DatabaseType.SqlAzure || databaseType == DatabaseType.SqlServer)
            {
                var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2008ProcessorFactory();
                processor = factory.Create(connectionString, announcer, options);
                dbConnection = ((SqlServerProcessor)processor).Connection;
            }
            else if (databaseType == DatabaseType.PostgreSQL)
            {
                var factory = new FluentMigrator.Runner.Processors.Postgres.PostgresProcessorFactory();
                processor = factory.Create(connectionString, announcer, options);
                dbConnection = ((PostgresProcessor)processor).Connection;
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                var factory = new FluentMigrator.Runner.Processors.Oracle.OracleProcessorFactory();
                processor = factory.Create(connectionString, announcer, options);
                dbConnection = ((OracleProcessor)processor).Connection;
            }
            else
            {
                throw new NotSupportedException(string.Format("Database type {0} is not supported for data migrations.", databaseType));
            }
            
            var runner = new MigrationRunner(assembly, migrationContext, processor);
            
            if (version != null)
            {
                runner.MigrateUp(version.Value);
            }
            else
            {
                throw new NotSupportedException("Migrations without target version are not supported.");
            }

            // If connection is still opened, close it.
            if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
            }
        }
    }
}
