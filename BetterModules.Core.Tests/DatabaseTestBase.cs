using System.Data.SqlClient;

using Autofac;

using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Tests.TestHelpers;
using BetterModules.Core.Tests.TestHelpers.Migrations;

namespace BetterModules.Core.Tests
{
    public abstract class DatabaseTestBase : TestBase
    {
        private static LocalDatabase database;
        
        private IRepository repository;

        private IUnitOfWork unitOfWork;

        protected SqlConnection SqlConnection
        {
            get { return database.SqlConnection; }
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
                InitializeDatabase();
            }
        }

        private void InitializeDatabase()
        {
            database = TestDatabaseInitializer.RunDatabaseMigrationTests();

            var configuration = Container.Resolve<IConfiguration>();
            configuration.Database.ConnectionString = database.ConnectionString;
        }

        protected override void OnTextFixtureDown()
        {
            if (database != null)
            {
                database.Dispose();
            }

            base.OnTextFixtureDown();
        }
    }
}
