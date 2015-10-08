using System.Data;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [Collection("Database test collection")]
    public class DefaultUnitOfWorkTests
    {
        private DatabaseTestFixture fixture;
        private readonly IRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public DefaultUnitOfWorkTests(DatabaseTestFixture fixture)
        {
            var provider = fixture.Services.BuildServiceProvider();
            repository = provider.GetService<IRepository>();
            unitOfWork = provider.GetService<IUnitOfWork>();
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Create_UoW_With_Session_Successfully()
        {
            var sessionFactoryProvider = fixture.Provider.GetService<ISessionFactoryProvider>();
            using (var session = sessionFactoryProvider.OpenSession())
            {
                using (var unitOfWork = new DefaultUnitOfWork(session))
                {
                    Assert.NotNull(unitOfWork.Session);
                }
            }
        }
        
        [Fact]
        public void Should_Create_UoW_With_SessionFactoryprovider_Successfully()
        {
            using (var unitOfWork = new DefaultUnitOfWork(fixture.Provider.GetService<ISessionFactoryProvider>()))
            {
                Assert.NotNull(unitOfWork.Session);
            }
        }

        [Fact]
        public void Should_Create_Transaction_Successfuly()
        {
            using (var unitOfWork = new DefaultUnitOfWork(fixture.Provider.GetService<ISessionFactoryProvider>()))
            {
                Assert.False(unitOfWork.IsActiveTransaction);
                unitOfWork.BeginTransaction();
                Assert.True(unitOfWork.IsActiveTransaction);
            }
        }
        
        [Fact]
        public void Should_Throw_Exception_Creating_Multiple_Transactions()
        {
            Assert.Throws<DataException>(() =>
            {
                using (var unitOfWork = new DefaultUnitOfWork(fixture.Provider.GetService<ISessionFactoryProvider>()))
                {
                    unitOfWork.BeginTransaction();
                    unitOfWork.BeginTransaction();
                }
            });
        }

        [Fact]
        public void Should_Rollback_Transaction_Successfully()
        {
            var model1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            var model2 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            try
            {
                unitOfWork.BeginTransaction();

                repository.Save(model1);
                repository.Save(model2);

                unitOfWork.Rollback();
            }
            catch
            {
                // Do nothing here
            }

            var loadedModel1 = repository.FirstOrDefault<TestItemModel>(model1.Id);
            var loadedModel2 = repository.FirstOrDefault<TestItemModel>(model2.Id);

            Assert.Null(loadedModel1);
            Assert.Null(loadedModel2);
        }
        
        [Fact]
        public void Should_Commit_And_Rollback_Transactions_Successfully()
        {
            var model1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            var model2 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            try
            {
                unitOfWork.BeginTransaction();
                repository.Save(model1);
                unitOfWork.Commit();

                unitOfWork.BeginTransaction();
                repository.Save(model2);
                unitOfWork.Rollback();
            }
            catch
            {
                // Do nothing here
            }

            var loadedModel1 = repository.FirstOrDefault<TestItemModel>(model1.Id);
            var loadedModel2 = repository.FirstOrDefault<TestItemModel>(model2.Id);

            Assert.NotNull(loadedModel1);
            Assert.Null(loadedModel2);
        }
    }
}
