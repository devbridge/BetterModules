using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Interceptors
{
    [Collection("Database test collection")]
    public class StaleInterceptorIntegrationTests
    {
        private DatabaseTestFixture fixture;
        private readonly IRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public StaleInterceptorIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
            var provider = fixture.Services.BuildServiceProvider();
            repository = provider.GetService<IRepository>();
            unitOfWork = provider.GetService<IUnitOfWork>();
        }

        [Fact]
        public void Should_Create_Version()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            
            Assert.Equal(model.Version, 0);

            repository.Save(model);
            unitOfWork.Commit();

            Assert.Equal(model.Version, 1);
        }
        
        [Fact]
        public void Should_Increase_Version_If_Dirty()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Assert.Equal(model.Version, 0);

            repository.Save(model);
            unitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            repository.Save(model);
            unitOfWork.Commit();

            Assert.Equal(model.Version, 1);

            model.Name = fixture.TestDataProvider.ProvideRandomString();
            repository.Save(model);
            unitOfWork.Commit();

            Assert.Equal(model.Version, 2);
        }

        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Saving()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                repository.Save(model);
                unitOfWork.Commit();

                model.Name = fixture.TestDataProvider.ProvideRandomString();
                model.Version = 3;

                repository.Save(model);
                unitOfWork.Commit();
            });
        }
        
        [Fact]
        public void Should_Throw_Concurrent_Data_Exception_Deleting()
        {
            Assert.Throws<ConcurrentDataException>(() =>
            {
                var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

                Assert.Equal(model.Version, 0);

                repository.Save(model);
                unitOfWork.Commit();

                model.Name = fixture.TestDataProvider.ProvideRandomString();
                model.Version = 3;

                repository.Delete(model);
                unitOfWork.Commit();
            });
        }
    }
}
