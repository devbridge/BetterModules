using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Exceptions;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Migrations
{
    [Collection("Database test collection")]
    public class DefaultMigrationTests
    {
        // TODO: Use Mocking instead of Fixture
        private DatabaseTestFixture fixture;

        public DefaultMigrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Throw_Core_Exception_Migrating_Down()
        {
            Assert.Throws<CoreException>(() =>
            {
                var migration = new TestMigration("Test");
                migration.Down();
            });
        }
        
        private class TestMigration : DefaultMigration
        {
            public TestMigration(string moduleName) : base(moduleName)
            {
            }

            public override void Up()
            {
            }
        }
    }
}
