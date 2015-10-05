using BetterModules.Core.DataAccess.DataContext;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    public class DefaultUnitOfWorkFactoryIntegrationTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Create_New_Unit_Of_Work()
        {
            var factory = new DefaultUnitOfWorkFactory(Provider.GetService<ISessionFactoryProvider>());
            var uow = factory.New();

            Assert.NotNull(uow);
            Assert.NotEqual(uow, UnitOfWork);
        }
    }
}
