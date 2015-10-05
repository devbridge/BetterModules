using System;
using System.Linq;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using NHibernate.Proxy.DynamicProxy;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess
{
    public class DefaultRepositoryIntegrationTests : IClassFixture<DatabaseTestBase>
    {
        private TestItemCategory category1;
        private TestItemModel model1;
        private TestItemModel model2;
        private TestItemModel model3;
        private TestItemModel deletedModel;
        private bool isSet;
        private DatabaseTestBase fixture;
        public DefaultRepositoryIntegrationTests(DatabaseTestBase fixture)
        {
            this.fixture = fixture;
            if (!isSet)
            {
                isSet = true;

                category1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();

                model1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model1.Name = "DRT_01";
                model2 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model2.Name = "DRT_02";
                model3 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model3.Name = "DRT_03";
                deletedModel = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                deletedModel.Name = "DRT_04";

                fixture.Repository.Save(model3);
                fixture.Repository.Save(deletedModel);
                fixture.Repository.Save(model2);
                fixture.Repository.Save(model1);
                fixture.UnitOfWork.Commit();

                fixture.Repository.Delete(deletedModel);
                fixture.UnitOfWork.Commit();
            }
        }

        [Fact]
        public void Should_UnProxy_Entity()
        {
            var proxy = fixture.Repository.AsProxy<TestItemModel>(SampleModuleDescriptor.TestItemModelId);
            Assert.NotNull(proxy);
            Assert.True(proxy is IProxy);

            var unproxy = fixture.Repository.UnProxy(proxy);
            Assert.NotNull(unproxy);
            Assert.Equal(unproxy.Id, SampleModuleDescriptor.TestItemModelId);
            Assert.False(unproxy is IProxy);
        }

        [Fact]
        public void Should_Return_Same_Entity_When_UnProxying_Entity()
        {
            var proxy = fixture.Repository.AsProxy<TestItemModel>(model1.Id);
            Assert.NotNull(proxy);

            var unproxy = fixture.Repository.UnProxy(proxy);
            Assert.NotNull(unproxy);

            Assert.Equal(unproxy, proxy);
        }

        [Fact]
        public void Should_Load_Entity_AsProxy()
        {
            var proxy = fixture.Repository.AsProxy<TestItemModel>(Guid.NewGuid());

            Assert.NotNull(proxy);
            Assert.True(proxy is IProxy);
        }

        [Fact]
        public void Should_Retrieve_First_Entity_By_Id()
        {
            var entity = fixture.Repository.First<TestItemModel>(model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Id()
        {
            Assert.Throws<EntityNotFoundException>(() =>
            {
                fixture.Repository.First<TestItemModel>(Guid.NewGuid());
            });

        }

        [Fact]
        public void Should_Retrieve_First_Entity_By_Filter()
        {
            var entity = fixture.Repository.First<TestItemModel>(m => m.Id == model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Filter()
        {
            Assert.Throws<EntityNotFoundException>(() =>
            {
                var guid = Guid.NewGuid();
                fixture.Repository.First<TestItemModel>(m => m.Id == guid);
            });
        }

        [Fact]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Id()
        {
            var entity = fixture.Repository.FirstOrDefault<TestItemModel>(model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Filter()
        {
            var entity = fixture.Repository.FirstOrDefault<TestItemModel>(m => m.Id == model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Id()
        {
            var guid = Guid.NewGuid();
            var entity = fixture.Repository.FirstOrDefault<TestItemModel>(guid);

            Assert.Null(entity);
        }

        [Fact]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Filter()
        {
            var guid = Guid.NewGuid();
            var entity = fixture.Repository.FirstOrDefault<TestItemModel>(m => m.Id == guid);

            Assert.Null(entity);
        }

        [Fact]
        public void Should_Return_QueryOver_Without_Deleted_Generic()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>().Where(q => q.Category == category1)
                .OrderBy(q => q.Name).Asc
                .List<TestItemModel>();

            Assert.NotNull(list);
            Assert.Equal(list.Count, 3);
            Assert.Equal(list[0].Id, model1.Id);
            Assert.Equal(list[1].Id, model2.Id);
            Assert.Equal(list[2].Id, model3.Id);
        }

        [Fact]
        public void Should_Return_QueryOver_Without_Deleted_By_Alias()
        {
            TestItemModel alias = null;
            var list = fixture.Repository
                .AsQueryOver(() => alias)
                .Where(() => alias.Category == category1)
                .OrderBy(q => q.Name).Asc
                .List<TestItemModel>();

            Assert.NotNull(list);
            Assert.Equal(list.Count, 3);
            Assert.Equal(list[0].Id, model1.Id);
            Assert.Equal(list[1].Id, model2.Id);
            Assert.Equal(list[2].Id, model3.Id);
        }

        [Fact]
        public void Should_Return_QueryOver_Without_Deleted_By_Null_Alias()
        {
            TestItemModel alias = null;
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>(null)
                .Where(t => t.Category == category1)
                .OrderBy(q => q.Name).Asc
                .List<TestItemModel>();

            Assert.NotNull(list);
            Assert.Equal(list.Count, 3);
            Assert.Equal(list[0].Id, model1.Id);
            Assert.Equal(list[1].Id, model2.Id);
            Assert.Equal(list[2].Id, model3.Id);
        }

        [Fact]
        public void Should_Return_Queryable_By_Filter()
        {
            var list = fixture.Repository
                .AsQueryable<TestItemModel>(q => q.Category == category1)
                .OrderBy(q => q.Name)
                .ToList();

            Assert.NotNull(list);
            Assert.Equal(list.Count, 3);
            Assert.Equal(list[0].Id, model1.Id);
            Assert.Equal(list[1].Id, model2.Id);
            Assert.Equal(list[2].Id, model3.Id);
        }

        [Fact]
        public void Should_Return_Queryable_Without_Deleted()
        {
            var list = fixture.Repository
                .AsQueryable<TestItemModel>()
                .Where(q => q.Category == category1)
                .OrderBy(q => q.Name)
                .ToList();

            Assert.NotNull(list);
            Assert.Equal(list.Count, 3);
            Assert.Equal(list[0].Id, model1.Id);
            Assert.Equal(list[1].Id, model2.Id);
            Assert.Equal(list[2].Id, model3.Id);
        }

        [Fact]
        public void Should_Check_If_Record_Exists()
        {
            var exists = fixture.Repository.Any<TestItemModel>(q => q.Name == model1.Name);

            Assert.True(exists);
        }

        [Fact]
        public void Should_Check_If_Deleted_Record_Not_Exists()
        {
            var exists = fixture.Repository.Any<TestItemModel>(q => q.Name == deletedModel.Name);

            Assert.False(exists);
        }

        [Fact]
        public void Should_Save_Entity()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.True(model.Id != default(Guid));
        }

        [Fact]
        public void Should_Delete_Entity_By_Id_NotAsProxy()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            fixture.Repository.Delete<TestItemModel>(model.Id, model.Version, false);
            fixture.UnitOfWork.Commit();

            var exists = fixture.Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Delete_Entity_By_Id_AsProxy()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            fixture.Repository.Delete<TestItemModel>(model.Id, model.Version, true);
            fixture.UnitOfWork.Commit();

            var exists = fixture.Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Delete_Entity()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            fixture.Repository.Delete(model);
            fixture.UnitOfWork.Commit();

            var exists = fixture.Repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Attach_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            var version = model.Version;

            // Load detached version, touch multiple times
            var detachedModel = fixture.Repository.First<TestItemModel>(model.Id);
            fixture.Repository.Detach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.UnitOfWork.Commit();
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.UnitOfWork.Commit();

            Assert.Equal(detachedModel.Version, version);

            // Attach and save again
            fixture.Repository.Attach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.UnitOfWork.Commit();

            Assert.NotEqual(detachedModel.Version, version);
        }

        [Fact]
        public void Should_Detach_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            // Touch entity - changes should be saved on flush
            var version = model.Version;

            var attachedModel = fixture.Repository.First<TestItemModel>(model.Id);
            attachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.UnitOfWork.Commit();

            Assert.True(attachedModel.Version > version);
            version = attachedModel.Version;

            // Detach and touch again - changes shouldn't saved on flush
            var detachedModel = fixture.Repository.First<TestItemModel>(model.Id);
            fixture.Repository.Detach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            fixture.UnitOfWork.Commit();

            Assert.Equal(detachedModel.Version, version);
        }

        [Fact]
        public void Should_Refresh_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            fixture.Repository.Save(model);
            fixture.UnitOfWork.Commit();

            var version = model.Version;

            // Load attached and detached version, touch multiple times
            var detachedModel = fixture.Repository.First<TestItemModel>(model.Id);

            // Open another session
            var provider = fixture.Services.BuildServiceProvider();
            var repository2 = provider.GetService<IRepository>();
            var unitOfWork2 = provider.GetService<IUnitOfWork>();

            var attachedModel = repository2.First<TestItemModel>(model.Id);
            attachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork2.Commit();

            Assert.Equal(detachedModel.Version, version);

            // Refresh detached entity - version should be updated
            fixture.Repository.Refresh(detachedModel);
            Assert.NotEqual(detachedModel.Version, version);
        }
    }
}
