using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Database.Tests.TestHelpers;
using BetterModules.Core.Database.Tests.TestHelpers.Migrations;
using BetterModules.Core.Extensions;
using BetterModules.Core.Tests.TestHelpers;
using BetterModules.Sample.Module;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Moq;

namespace BetterModules.Core.Database.Tests
{
    public class DatabaseTestBase: IDisposable
    {
        private DatabaseRandomTestDataProvider dbTestDataProvider;

        private RandomTestDataProvider testDataProvider;

        private LocalDatabase database;

        private bool started;

        private IRepository repository;

        private IUnitOfWork unitOfWork;

        public IServiceCollection Services { get; set; }

        public IServiceProvider Provider { get; set; }

        public SqlConnection SqlConnection => database.SqlConnection;

        public IRepository Repository => repository ?? (repository = Provider.GetService<IRepository>());

        public IUnitOfWork UnitOfWork => unitOfWork ?? (unitOfWork = Provider.GetService<IUnitOfWork>());

        public DatabaseTestBase()
        {
            if (!started)
            {
                InitializeServices();
                started = true;
            }
            if (database == null)
            {
                InitializeDatabase();
            }
        }

        private void InitializeServices()
        {
            var manager = new Mock<ILibraryManager>();
            manager.Setup(m => m.GetReferencingLibraries("BetterModules.Core"))
                .Returns(new[]
                {
                    new Library("SampleModule", "", "", "", new[] {"BetterModules.Core"}, new List<AssemblyName>
                    {
                        Assembly.GetAssembly(typeof(SampleModuleDescriptor)).GetName()
                    })
                });
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddInstance(manager.Object);
            services.AddOptions();
            var builder = new ConfigurationBuilder(System.Environment.CurrentDirectory)
                .AddJsonFile("Config/modules.json")
                .AddJsonFile("Config/connectionStrings.json");
            services.AddBetterModulesCore(builder.Build());
            Services = services;
            Provider = services.BuildServiceProvider();
        }

        private void InitializeDatabase()
        {
            database = TestDatabaseInitializer.RunDatabaseMigrationTests(Provider);

            var configuration = Provider.GetService<IOptions<DefaultConfigurationSection>>().Options;
            configuration.Database.ConnectionString = database.ConnectionString;
        }

        public virtual DatabaseRandomTestDataProvider DatabaseTestDataProvider => dbTestDataProvider ?? (dbTestDataProvider = new DatabaseRandomTestDataProvider());

        public virtual RandomTestDataProvider TestDataProvider => testDataProvider ?? (testDataProvider = new RandomTestDataProvider());
        public void Dispose()
        {
            database?.Dispose();
        }
    }
}
