using Autofac;
using BetterModules.Core.DataAccess.DataContext;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class DefaultUnitOfWorkFactoryIntegrationTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Create_New_Unit_Of_Work()
        {
            var factory = new DefaultUnitOfWorkFactory(Container.Resolve<ISessionFactoryProvider>());
            var uow = factory.New();

            Assert.IsNotNull(uow);
            Assert.AreNotEqual(uow, UnitOfWork);
        }
    }
}
