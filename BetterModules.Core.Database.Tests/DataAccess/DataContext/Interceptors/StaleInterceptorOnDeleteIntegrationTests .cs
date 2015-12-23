using System;
using BetterModules.Core.Exceptions.DataTier;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Interceptors
{
    [TestFixture]
    public class StaleInterceptorIntegrationTests2 : DatabaseTestBase
    {        
        [Test]
        public void Should_Throw_Concurrent_Data_Exception_Deleting()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            model.Name = TestDataProvider.ProvideRandomString();
            model.Version = 3;

            Assert.Throws<ConcurrentDataException>(() =>
            {
                Repository.Delete(model);
                UnitOfWork.Commit();
            });
        }
    }
}
