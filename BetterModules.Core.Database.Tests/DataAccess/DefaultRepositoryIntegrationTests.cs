using System;
using System.Linq;
using Autofac;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module;
using BetterModules.Sample.Module.Models;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess
{
    [TestFixture]
    public class DefaultRepositoryIntegrationTests : DatabaseTestBase
    {
        private TestItemCategory category1;
        private TestItemModel model1;
        private TestItemModel model2;
        private TestItemModel model3;
        private TestItemModel deletedModel;
        private bool isSet;

        [SetUp]
        public void SetUp()
        {
            if (!isSet)
            {
                isSet = true;

                category1 = DatabaseTestDataProvider.ProvideRandomTestItemCategory();

                model1 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model1.Name = "DRT_01";
                model2 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model2.Name = "DRT_02";
                model3 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model3.Name = "DRT_03";
                deletedModel = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                deletedModel.Name = "DRT_04";

                Repository.Save(model3);
                Repository.Save(deletedModel);
                Repository.Save(model2);
                Repository.Save(model1);
                UnitOfWork.Commit();

                Repository.Delete(deletedModel);
                UnitOfWork.Commit();
            }
        }

        [Test]
        public void Should_UnProxy_Entity()
        {
            var proxy = Repository.AsProxy<TestItemModel>(SampleModuleDescriptor.TestItemModelId);
            Assert.IsNotNull(proxy);
            Assert.IsTrue(proxy is IProxy);

            var unproxy = Repository.UnProxy(proxy);
            Assert.IsNotNull(unproxy);
            Assert.AreEqual(unproxy.Id, SampleModuleDescriptor.TestItemModelId);
            Assert.IsFalse(unproxy is IProxy);
        }
        
        [Test]
        public void Should_Return_Same_Entity_When_UnProxying_Entity()
        {
            var proxy = Repository.AsProxy<TestItemModel>(model1.Id);
            Assert.IsNotNull(proxy);

            var unproxy = Repository.UnProxy(proxy);
            Assert.IsNotNull(unproxy);
            
            Assert.AreEqual(unproxy, proxy);
        }
        
        [Test]
        public void Should_Load_Entity_AsProxy()
        {
            var proxy = Repository.AsProxy<TestItemModel>(Guid.NewGuid());

            Assert.IsNotNull(proxy);
            Assert.IsTrue(proxy is IProxy);
        }
        
        [Test]
        public void Should_Retrieve_First_Entity_By_Id()
        {
            var entity = Repository.First<TestItemModel>(model1.Id);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, model1.Id);
        }
        
        [Test]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Id()
        {
            Repository.First<TestItemModel>(Guid.NewGuid());
        }

        [Test]
        public void Should_Retrieve_First_Entity_By_Filter()
        {
            var entity = Repository.First<TestItemModel>(m => m.Id == model1.Id);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, model1.Id);
        }

        [Test]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Filter()
        {
            var guid = Guid.NewGuid();
            Repository.First<TestItemModel>(m => m.Id == guid);
        }

        [Test]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Id()
        {
            var entity = Repository.FirstOrDefault<TestItemModel>(model1.Id);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, model1.Id);
        }
        
        [Test]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Filter()
        {
            var entity = Repository.FirstOrDefault<TestItemModel>(m => m.Id == model1.Id);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, model1.Id);
        }
        
        [Test]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Id()
        {
            var guid = Guid.NewGuid();
            var entity = Repository.FirstOrDefault<TestItemModel>(guid);

            Assert.IsNull(entity);
        }
        
        [Test]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Filter()
        {
            var guid = Guid.NewGuid();
            var entity = Repository.FirstOrDefault<TestItemModel>(m => m.Id == guid);

            Assert.IsNull(entity);
        }

        [Test]
        public void Should_Return_QueryOver_Without_Deleted_Generic()
        {
            var list = Repository
                .AsQueryOver<TestItemModel>().Where(q => q.Category == category1)
                .OrderBy(q => q.Name).Asc
                .List<TestItemModel>();

            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].Id, model1.Id);
            Assert.AreEqual(list[1].Id, model2.Id);
            Assert.AreEqual(list[2].Id, model3.Id);
        }

        [Test]
        public void Should_Return_QueryOver_Without_Deleted_By_Alias()
        {
            TestItemModel alias = null;
            var list = Repository
                .AsQueryOver(() => alias)
                .Where(() => alias.Category == category1)
                .OrderBy(q => q.Name).Asc
                .List<TestItemModel>();

            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].Id, model1.Id);
            Assert.AreEqual(list[1].Id, model2.Id);
            Assert.AreEqual(list[2].Id, model3.Id);
        }

        [Test]
        public void Should_Return_Queryable_By_Filter()
        {
            var list = Repository
                .AsQueryable<TestItemModel>(q => q.Category == category1)
                .OrderBy(q => q.Name)
                .ToList();

            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].Id, model1.Id);
            Assert.AreEqual(list[1].Id, model2.Id);
            Assert.AreEqual(list[2].Id, model3.Id);
        }
        
        [Test]
        public void Should_Return_Queryable_Without_Deleted()
        {
            var list = Repository
                .AsQueryable<TestItemModel>()
                .Where(q => q.Category == category1)
                .OrderBy(q => q.Name)
                .ToList();

            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].Id, model1.Id);
            Assert.AreEqual(list[1].Id, model2.Id);
            Assert.AreEqual(list[2].Id, model3.Id);
        }

        [Test]
        public void Should_Check_If_Record_Exists()
        {
            var exists = Repository.Any<TestItemModel>(q => q.Name == model1.Name);

            Assert.IsTrue(exists);
        }
        
        [Test]
        public void Should_Check_If_Deleted_Record_Not_Exists()
        {
            var exists = Repository.Any<TestItemModel>(q => q.Name == deletedModel.Name);

            Assert.IsFalse(exists);
        }

        [Test]
        public void Should_Save_Entity()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            
            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.IsTrue(model.Id != default(Guid));
        }

        [Test]
        public void Should_Delete_Entity_By_Id_NotAsProxy()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.IsTrue(model.Id != default(Guid));

            Repository.Delete<TestItemModel>(model.Id, model.Version, false);
            UnitOfWork.Commit();

            var exists = Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.IsFalse(exists);
        }
        
        [Test]
        public void Should_Delete_Entity_By_Id_AsProxy()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.IsTrue(model.Id != default(Guid));

            Repository.Delete<TestItemModel>(model.Id, model.Version, true);
            UnitOfWork.Commit();

            var exists = Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Should_Delete_Entity()
        {
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();

            Repository.Save(model);
            UnitOfWork.Commit();

            Assert.IsTrue(model.Id != default(Guid));

            Repository.Delete(model);
            UnitOfWork.Commit();

            var exists = Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Should_Attach_Entity()
        {
            // Create entity
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(model);
            UnitOfWork.Commit();

            var version = model.Version;

            // Load detached version, touch multiple times
            var detachedModel = Repository.First<TestItemModel>(model.Id);
            Repository.Detach(detachedModel);
            detachedModel.Name = TestDataProvider.ProvideRandomString();
            UnitOfWork.Commit();
            detachedModel.Name = TestDataProvider.ProvideRandomString();
            UnitOfWork.Commit();

            Assert.AreEqual(detachedModel.Version, version);

            // Attach and save again
            Repository.Attach(detachedModel);
            detachedModel.Name = TestDataProvider.ProvideRandomString();
            UnitOfWork.Commit();

            Assert.AreNotEqual(detachedModel.Version, version);
        }

        [Test]
        public void Should_Detach_Entity()
        {
            // Create entity
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(model);
            UnitOfWork.Commit();

            // Touch entity - changes should be saved on flush
            var version = model.Version;
            
            var attachedModel = Repository.First<TestItemModel>(model.Id);
            attachedModel.Name = TestDataProvider.ProvideRandomString();
            UnitOfWork.Commit();

            Assert.Greater(attachedModel.Version, version);
            version = attachedModel.Version;

            // Detach and touch again - changes shouldn't saved on flush
            var detachedModel = Repository.First<TestItemModel>(model.Id);
            Repository.Detach(detachedModel);
            detachedModel.Name = TestDataProvider.ProvideRandomString();
            UnitOfWork.Commit();

            Assert.AreEqual(detachedModel.Version, version);
        }

        [Test]
        public void Should_Refresh_Entity()
        {
            // Create entity
            var model = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(model);
            UnitOfWork.Commit();

            var version = model.Version;

            // Load attached and detached version, touch multiple times
            var detachedModel = Repository.First<TestItemModel>(model.Id);

            // Open another session
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var repository2 = container.Resolve<IRepository>();
                var unitOfWork2 = container.Resolve<IUnitOfWork>();

                var attachedModel = repository2.First<TestItemModel>(model.Id);
                attachedModel.Name = TestDataProvider.ProvideRandomString();
                unitOfWork2.Commit();
            }

            Assert.AreEqual(detachedModel.Version, version);

            // Refresh detached entity - version should be updated
            Repository.Refresh(detachedModel);
            Assert.AreNotEqual(detachedModel.Version, version);
        }
    }
}
