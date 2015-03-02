using System;
using Autofac;
using BetterModules.Core.Security;
using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext.EventListeners
{
    [TestFixture]
    public class DeleteEventListenerIntegrationTests : DatabaseTestBase
    {
        [Test]
        public void Should_Mark_Entity_As_Deleted()
        {
            var entity = TestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            Assert.AreNotSame(entity.Id.ToString(), Guid.Empty.ToString());
            Repository.Delete(entity);
            UnitOfWork.Commit();

            var principalProvider = Container.Resolve<IPrincipalProvider>();

            Assert.IsTrue(entity.IsDeleted);
            Assert.IsNotNull(entity.DeletedOn);
            Assert.AreEqual(entity.DeletedByUser, principalProvider.CurrentPrincipalName);
        }
    }
}
