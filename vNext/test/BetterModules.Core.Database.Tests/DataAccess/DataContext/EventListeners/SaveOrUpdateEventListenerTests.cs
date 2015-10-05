using Autofac;
using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [TestFixture]
    public class SaveOrUpdateEventListenerTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Update_Entity_Properties_When_Creating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Container.Resolve<IPrincipalProvider>();

            Assert.IsNotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);
        }
        
        [Fact]
        public void Should_Update_Entity_Properties_When_Updating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Container.Resolve<IPrincipalProvider>();

            Assert.IsNotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            var modified = entity.ModifiedOn;

            var loadedEntity = Repository.FirstOrDefault<TestItemModel>(entity.Id);
            loadedEntity.Name = TestDataProvider.ProvideRandomString(100);
            Repository.Save(loadedEntity);
            UnitOfWork.Commit();

            Assert.IsNotNull(loadedEntity.CreatedOn);
            Assert.Equal(loadedEntity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(loadedEntity.ModifiedOn);
            Assert.Equal(loadedEntity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            Assert.Equal(loadedEntity.CreatedOn, entity.CreatedOn);
            Assert.AreNotEqual(loadedEntity.ModifiedOn, modified);
        }
    }
}
