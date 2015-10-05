using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    public class SaveOrUpdateEventListenerTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Update_Entity_Properties_When_Creating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);
        }
        
        [Fact]
        public void Should_Update_Entity_Properties_When_Updating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            var modified = entity.ModifiedOn;

            var loadedEntity = Repository.FirstOrDefault<TestItemModel>(entity.Id);
            loadedEntity.Name = TestDataProvider.ProvideRandomString(100);
            Repository.Save(loadedEntity);
            UnitOfWork.Commit();

            Assert.NotNull(loadedEntity.CreatedOn);
            Assert.Equal(loadedEntity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(loadedEntity.ModifiedOn);
            Assert.Equal(loadedEntity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            Assert.Equal(loadedEntity.CreatedOn, entity.CreatedOn);
            Assert.NotEqual(loadedEntity.ModifiedOn, modified);
        }
    }
}
