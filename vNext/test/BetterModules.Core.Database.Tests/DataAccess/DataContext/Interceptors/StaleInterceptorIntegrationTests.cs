using BetterModules.Core.Exceptions.DataTier;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Interceptors
{
    [Collection("Database test collection")]
    public class StaleInterceptorIntegrationTests
    {
        private DatabaseTestFixture fixture;

        public StaleInterceptorIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Create_Version()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            
            Assert.Equal(model.Version, 0);

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);
        }
        
        [Fact]
        public void Should_Increase_Version_If_Dirty()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.Equal(model.Version, 0);

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            model.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.Equal(model.Version, 2);
        }

        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Saving()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                fixture.Repository.Save(model);
                fixture.UnitOfWork.Commit();

                model.Name = fixture.TestDataProvider.ProvideRandomString();
                model.Version = 3;

                fixture.Repository.Save(model);
                fixture.UnitOfWork.Commit();
            });
        }
        
        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Deleting()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                fixture.Repository.Save(model);
                fixture.UnitOfWork.Commit();

                model.Name = fixture.TestDataProvider.ProvideRandomString();
                model.Version = 3;

                fixture.Repository.Delete(model);
                fixture.UnitOfWork.Commit();
            });
        }
    }
}
