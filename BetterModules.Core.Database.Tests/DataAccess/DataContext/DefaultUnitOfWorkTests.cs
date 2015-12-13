using System.Data;
using Autofac;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class DefaultUnitOfWorkTests : DatabaseTestBase
    {
        [Test]
        public void Should_Create_UoW_With_Session_Successfully()
        {
            var sessionFactoryProvider = Container.Resolve<ISessionFactoryProvider>();
            using (var session = sessionFactoryProvider.OpenSession())
            {
                using (var unitOfWork = new DefaultUnitOfWork(session))
                {
                    Assert.IsNotNull(unitOfWork.Session);
                }
            }
        }
        
        [Test]
        public void Should_Create_UoW_With_SessionFactoryprovider_Successfully()
        {
            using (var unitOfWork = new DefaultUnitOfWork(Container.Resolve<ISessionFactoryProvider>()))
            {
                Assert.IsNotNull(unitOfWork.Session);
            }
        }

        [Test]
        public void Should_Create_Transaction_Successfuly()
        {
            using (var unitOfWork = new DefaultUnitOfWork(Container.Resolve<ISessionFactoryProvider>()))
            {
                Assert.IsFalse(unitOfWork.IsActiveTransaction);
                unitOfWork.BeginTransaction();
                Assert.IsTrue(unitOfWork.IsActiveTransaction);
            }
        }
        
        [Test]
        public void Should_Throw_Exception_Creating_Multiple_Transactions()
        {
            Assert.Throws<DataException>(() =>
            {
                using (var unitOfWork = new DefaultUnitOfWork(Container.Resolve<ISessionFactoryProvider>()))
                {
                    unitOfWork.BeginTransaction();
                    unitOfWork.BeginTransaction();
                }
            });
        }

        [Test]
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

            Assert.IsNull(loadedModel1);
            Assert.IsNull(loadedModel2);
        }
        
        [Test]
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

            Assert.IsNotNull(loadedModel1);
            Assert.IsNull(loadedModel2);
        }
    }
}
