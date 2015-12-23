using System;
using BetterModules.Core.Exceptions.DataTier;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Interceptors
{
    [TestFixture]
    public class StaleInterceptorIntegrationTests : DatabaseTestBase
    {   
        [Test]
        public void Should_Create_Version()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            
            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(model.Version, 1);
        }
        
        [Test]
        public void Should_Increase_Version_If_Dirty()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(model.Version, 1);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(model.Version, 1);

            model.Name = TestDataProvider.ProvideRandomString();
            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(model.Version, 2);
        }

        [Test]
        public void Should_Throw_Concurrent_Data_Exception_Saving()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.AreEqual(0, model.Version);
            Assert.AreEqual(0, model.Category.Version);
        
            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(1, model.Version);
            Assert.AreEqual(1, model.Category.Version);

            model.Name = TestDataProvider.ProvideRandomString();
            model.Version = 3;

            Assert.Throws<ConcurrentDataException>(() =>
            {
                Repository.Save(model);
                UnitOfWork.Commit();
            });
        }
     
    }
}
