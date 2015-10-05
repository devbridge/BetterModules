using System.Data;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    public class DefaultUnitOfWorkTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Create_UoW_With_Session_Successfully()
        {
            var sessionFactoryProvider = Provider.GetService<ISessionFactoryProvider>();
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
            using (var unitOfWork = new DefaultUnitOfWork(Provider.GetService<ISessionFactoryProvider>()))
            {
                Assert.NotNull(unitOfWork.Session);
            }
        }

        [Fact]
        public void Should_Create_Transaction_Successfuly()
        {
            using (var unitOfWork = new DefaultUnitOfWork(Provider.GetService<ISessionFactoryProvider>()))
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
                using (var unitOfWork = new DefaultUnitOfWork(Provider.GetService<ISessionFactoryProvider>()))
                {
                    unitOfWork.BeginTransaction();
                    unitOfWork.BeginTransaction();
                }
            });
        }

        [Fact]
        public void Should_Rollback_Transaction_Successfully()
        {
            var model1 = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            var model2 = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            try
            {
                UnitOfWork.BeginTransaction();

                Repository.Save(model1);
                Repository.Save(model2);

                UnitOfWork.Rollback();
            }
            catch
            {
                // Do nothing here
            }

            var loadedModel1 = Repository.FirstOrDefault<TestItemModel>(model1.Id);
            var loadedModel2 = Repository.FirstOrDefault<TestItemModel>(model2.Id);

            Assert.Null(loadedModel1);
            Assert.Null(loadedModel2);
        }
        
        [Fact]
        public void Should_Commit_And_Rollback_Transactions_Successfully()
        {
            var model1 = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            var model2 = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            try
            {
                UnitOfWork.BeginTransaction();
                Repository.Save(model1);
                UnitOfWork.Commit();

                UnitOfWork.BeginTransaction();
                Repository.Save(model2);
                UnitOfWork.Rollback();
            }
            catch
            {
                // Do nothing here
            }

            var loadedModel1 = Repository.FirstOrDefault<TestItemModel>(model1.Id);
            var loadedModel2 = Repository.FirstOrDefault<TestItemModel>(model2.Id);

            Assert.NotNull(loadedModel1);
            Assert.Null(loadedModel2);
        }
    }
}
