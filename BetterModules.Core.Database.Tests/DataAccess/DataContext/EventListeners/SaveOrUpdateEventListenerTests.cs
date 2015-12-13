using System;
using System.Threading;
using Autofac;
using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [TestFixture]
    public class SaveOrUpdateEventListenerTests : DatabaseTestBase
    {
        [Test]
        public void Should_Update_Entity_Properties_When_Creating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Container.Resolve<IPrincipalProvider>();

            Assert.IsNotNull(entity.CreatedOn);
            Assert.AreEqual(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(entity.ModifiedOn);
            Assert.AreEqual(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);
        }
        
        [Test]
        public void Should_Update_Entity_Properties_When_Updating()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            var principalProvider = Container.Resolve<IPrincipalProvider>();

            Assert.IsNotNull(entity.CreatedOn);
            Assert.AreEqual(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(entity.ModifiedOn);
            Assert.AreEqual(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            var modified = new DateTime(entity.ModifiedOn.Ticks);
            Thread.Sleep(25);

            var loadedEntity = Repository.FirstOrDefault<TestItemModel>(entity.Id);
            loadedEntity.Name = TestDataProvider.ProvideRandomString(100);
            Repository.Save(loadedEntity);
            UnitOfWork.Commit();

            Assert.IsNotNull(loadedEntity.CreatedOn);
            Assert.AreEqual(loadedEntity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.IsNotNull(loadedEntity.ModifiedOn);
            Assert.AreEqual(loadedEntity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            Assert.AreEqual(loadedEntity.CreatedOn, entity.CreatedOn);
            Assert.AreNotEqual(loadedEntity.ModifiedOn, modified);
        }
    }
}
