using BetterModules.Core.Exceptions.DataTier;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Interceptors
{
    public class StaleInterceptorIntegrationTests : DatabaseTestBase
    {   
        [Fact]
        public void Should_Create_Version()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            
            Assert.Equal(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);
        }
        
        [Fact]
        public void Should_Increase_Version_If_Dirty()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.Equal(model.Version, 0);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            model.Name = TestDataProvider.ProvideRandomString();
            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.Equal(model.Version, 2);
        }

        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Saving()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                Repository.Save(model);
                UnitOfWork.Commit();

                model.Name = TestDataProvider.ProvideRandomString();
                model.Version = 3;

                Repository.Save(model);
                UnitOfWork.Commit();
            });
        }
        
        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Deleting()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                Repository.Save(model);
                UnitOfWork.Commit();

                model.Name = TestDataProvider.ProvideRandomString();
                model.Version = 3;

                Repository.Delete(model);
                UnitOfWork.Commit();
            });
        }
    }
}
