using System;
using Microsoft.Framework.DependencyInjection;
using BetterModules.Core.Security;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [Collection("Database test collection")]
    public class DeleteEventListenerIntegrationTests
    {
        private DatabaseTestFixture fixture;

        public DeleteEventListenerIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Mark_Entity_As_Deleted()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(entity);
            fixture.UnitOfWork.Commit();

            Assert.NotSame(entity.Id.ToString(), Guid.Empty.ToString());
            fixture.Repository.Delete(entity);
            fixture.UnitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.True(entity.IsDeleted);
            Assert.NotNull(entity.DeletedOn);
            Assert.Equal(entity.DeletedByUser, principalProvider.CurrentPrincipalName);
        }
    }
}
