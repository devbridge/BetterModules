using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [Collection("Database test collection")]
    public class SaveOrUpdateEventListenerTests
    {
        private DatabaseTestFixture fixture;

        public SaveOrUpdateEventListenerTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Update_Entity_Properties_When_Creating()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(entity);
            fixture.UnitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);
        }
        
        [Fact]
        public void Should_Update_Entity_Properties_When_Updating()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(entity);
            fixture.UnitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            var modified = entity.ModifiedOn;

            var loadedEntity = fixture.Repository.FirstOrDefault<TestItemModel>(entity.Id);
            loadedEntity.Name = fixture.TestDataProvider.ProvideRandomString(100);
            fixture.Repository.Save(loadedEntity);
            fixture.UnitOfWork.Commit();

            Assert.NotNull(loadedEntity.CreatedOn);
            Assert.Equal(loadedEntity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(loadedEntity.ModifiedOn);
            Assert.Equal(loadedEntity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            Assert.Equal(loadedEntity.CreatedOn, entity.CreatedOn);
            Assert.NotEqual(loadedEntity.ModifiedOn, modified);
        }
    }
}
