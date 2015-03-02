using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Tests.TestHelpers;
using BetterModules.Core.Tests.TestHelpers.Migrations;
using NUnit.Framework;

namespace BetterModules.Core.Tests
{
    public abstract class DatabaseTestBase : TestBase
    {
        private static LocalDatabase database;

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

            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var configuration = container.Resolve<IConfiguration>();
                configuration.Database.ConnectionString = database.ConnectionString;
            }
        }

        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            if (database != null)
            {
                database.Dispose();
            }
        }
    }
}
