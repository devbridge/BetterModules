using System.Data.SqlClient;
using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Database.Tests.TestHelpers;
using BetterModules.Core.Database.Tests.TestHelpers.Migrations;
using BetterModules.Core.Tests;

namespace BetterModules.Core.Database.Tests
{
    public abstract class DatabaseTestBase : TestBase
    {
        private DatabaseRandomTestDataProvider testDataProvider;

        private static readonly object lockObject = new object();

        private volatile static LocalDatabase database;
        
        private IRepository repository;

        private IUnitOfWork unitOfWork;

        protected SqlConnection SqlConnection
        {
            get
            {
                if (database == null)
                {
                    lock (lockObject)
                    {
                        if (database == null)
                        {
                            InitializeDatabase();
                        }
                    }
                }

                return database.SqlConnection;
            }
        }

        protected IRepository Repository
        {
            get
            {
                return repository ?? (repository = Container.Resolve<IRepository>());
            }
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                return unitOfWork ?? (unitOfWork = Container.Resolve<IUnitOfWork>());
            }
        }

        protected DatabaseTestBase()
        {
            if (database == null)
            {
                lock (lockObject)
                {
                    if (database == null)
                    {
                        InitializeDatabase();
                    }
                }
            }
        }

        private void InitializeDatabase()
        {
            database = TestDatabaseInitializer.RunDatabaseMigrationTests();

            var configuration = Container.Resolve<IConfiguration>();
            configuration.Database.ConnectionString = database.ConnectionString;
        }

        protected override void OnTestFixtureDown()
        {

            if (database != null)
            {
            //    database.Dispose();
            }

            base.OnTestFixtureDown();
        }

        public virtual DatabaseRandomTestDataProvider DatabaseTestDataProvider
        {
            get
            {
                if (testDataProvider == null)
                {
                    testDataProvider = new DatabaseRandomTestDataProvider();
                }
                return testDataProvider;
            }
        }
    }
}
