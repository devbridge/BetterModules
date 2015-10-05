using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Exceptions;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Migrations
{
    [TestFixture]
    public class DefaultMigrationTests
    {
        [Fact]
        [ExpectedException(typeof(CoreException))]
        public void Should_Throw_Core_Exception_Migrating_Down()
        {
            var migration = new TestMigration("Test");
            migration.Down();
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
