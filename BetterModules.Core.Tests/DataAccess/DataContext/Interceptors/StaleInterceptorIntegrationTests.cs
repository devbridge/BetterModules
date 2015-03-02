using BetterModules.Core.Exceptions.DataTier;
using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext.Interceptors
{
    [TestFixture]
    public class StaleInterceptorIntegrationTests : DatabaseTestBase
    {   
        [Test]
        public void Should_Create_Version()
        {
            var model = TestDataProvider.ProvideRandomTestItemModel();
            
            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.AreEqual(model.Version, 1);
        }
        
        [Test]
        public void Should_Increase_Version_If_Dirty()
        {
            var model = TestDataProvider.ProvideRandomTestItemModel();

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
        [ExpectedException(typeof (ConcurrentDataException))]
        public void Should_Throw_Concurrent_Data_Exception_Saving()
        {
            var model = TestDataProvider.ProvideRandomTestItemModel();

            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            model.Name = TestDataProvider.ProvideRandomString();
            model.Version = 3;

            Repository.Save(model);
            UnitOfWork.Commit();
        }
        
        [Test]
        [ExpectedException(typeof (ConcurrentDataException))]
        public void Should_Throw_Concurrent_Data_Exception_Deleting()
        {
            var model = TestDataProvider.ProvideRandomTestItemModel();

            Assert.AreEqual(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            model.Name = TestDataProvider.ProvideRandomString();
            model.Version = 3;

            Repository.Delete(model);
            UnitOfWork.Commit();
        }
    }
}
