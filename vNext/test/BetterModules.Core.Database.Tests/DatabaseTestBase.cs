using System;
using System.Data.SqlClient;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Database.Tests.TestHelpers;
using BetterModules.Core.Database.Tests.TestHelpers.Migrations;
using BetterModules.Core.Extensions;
using BetterModules.Core.Tests.TestHelpers;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace BetterModules.Core.Database.Tests
{
    public abstract class DatabaseTestBase: IDisposable
    {
        private DatabaseRandomTestDataProvider dbTestDataProvider;

        private RandomTestDataProvider testDataProvider;

        private static LocalDatabase database;
        
        private IRepository repository;

        private IUnitOfWork unitOfWork;

        protected IServiceCollection Services { get; set; }

        protected IServiceProvider Provider { get; set; }

        protected SqlConnection SqlConnection => database.SqlConnection;

        protected IRepository Repository => repository ?? (repository = Provider.GetService<IRepository>());

        protected IUnitOfWork UnitOfWork => unitOfWork ?? (unitOfWork = Provider.GetService<IUnitOfWork>());

        protected DatabaseTestBase()
        {
            InitializeServices();
            if (database == null)
            {
                InitializeDatabase();
            }
        }

        private void InitializeServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<ILibraryManager, LibraryManager>();
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
