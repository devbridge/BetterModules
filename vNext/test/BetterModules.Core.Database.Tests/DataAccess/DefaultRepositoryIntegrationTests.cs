using System;
using System.Linq;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using NHibernate.Proxy.DynamicProxy;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess
{
    [Collection("Database test collection")]
    public class DefaultRepositoryIntegrationTests
    {
        private TestItemCategory category1;
        private TestItemModel model1;
        private TestItemModel model2;
        private TestItemModel model3;
        private TestItemModel deletedModel;
        private bool isSet;
        private DatabaseTestFixture fixture;
        private readonly IRepository repository;
        private readonly IUnitOfWork unitOfWork;
        public DefaultRepositoryIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
            var provider = fixture.Services.BuildServiceProvider();
            repository = provider.GetService<IRepository>();
            unitOfWork = provider.GetService<IUnitOfWork>();
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

                repository.Save(model3);
                repository.Save(deletedModel);
                repository.Save(model2);
                repository.Save(model1);
                unitOfWork.Commit();

                repository.Delete(deletedModel);
                unitOfWork.Commit();
            }
        }

        [Fact]
        public void Should_UnProxy_Entity()
        {
            var proxy = repository.AsProxy<TestItemModel>(SampleModuleDescriptor.TestItemModelId);
            Assert.NotNull(proxy);
            Assert.True(proxy is IProxy);

            var unproxy = repository.UnProxy(proxy);
            Assert.NotNull(unproxy);
            Assert.Equal(unproxy.Id, SampleModuleDescriptor.TestItemModelId);
            Assert.False(unproxy is IProxy);
        }

        [Fact]
        public void Should_Return_Same_Entity_When_UnProxying_Entity()
        {
            var proxy = repository.AsProxy<TestItemModel>(model1.Id);
            Assert.NotNull(proxy);

            var unproxy = repository.UnProxy(proxy);
            Assert.NotNull(unproxy);

            Assert.Equal(unproxy, proxy);
        }

        [Fact]
        public void Should_Load_Entity_AsProxy()
        {
            var proxy = repository.AsProxy<TestItemModel>(Guid.NewGuid());

            Assert.NotNull(proxy);
            Assert.True(proxy is IProxy);
        }

        [Fact]
        public void Should_Retrieve_First_Entity_By_Id()
        {
            var entity = repository.First<TestItemModel>(model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Id()
        {
            Assert.Throws<EntityNotFoundException>(() =>
            {
                repository.First<TestItemModel>(Guid.NewGuid());
            });

        }

        [Fact]
        public void Should_Retrieve_First_Entity_By_Filter()
        {
            var entity = repository.First<TestItemModel>(m => m.Id == model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Throw_Exception_Retrieving_First_Entity_By_Filter()
        {
            Assert.Throws<EntityNotFoundException>(() =>
            {
                var guid = Guid.NewGuid();
                repository.First<TestItemModel>(m => m.Id == guid);
            });
        }

        [Fact]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Id()
        {
            var entity = repository.FirstOrDefault<TestItemModel>(model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Retrieve_FirstOrDefault_Entity_By_Filter()
        {
            var entity = repository.FirstOrDefault<TestItemModel>(m => m.Id == model1.Id);

            Assert.NotNull(entity);
            Assert.Equal(entity.Id, model1.Id);
        }

        [Fact]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Id()
        {
            var guid = Guid.NewGuid();
            var entity = repository.FirstOrDefault<TestItemModel>(guid);

            Assert.Null(entity);
        }

        [Fact]
        public void Should_Retrieve_Null_Retrieving_FirstOrDefault_Entity_By_Filter()
        {
            var guid = Guid.NewGuid();
            var entity = repository.FirstOrDefault<TestItemModel>(m => m.Id == guid);

            Assert.Null(entity);
        }

        [Fact]
        public void Should_Return_QueryOver_Without_Deleted_Generic()
        {
            var list = repository
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
            var list = repository
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
            var list = repository
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
            var list = repository
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
            var list = repository
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
            var exists = repository.Any<TestItemModel>(q => q.Name == model1.Name);

            Assert.True(exists);
        }

        [Fact]
        public void Should_Check_If_Deleted_Record_Not_Exists()
        {
            var exists = repository.Any<TestItemModel>(q => q.Name == deletedModel.Name);

            Assert.False(exists);
        }

        [Fact]
        public void Should_Save_Entity()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            repository.Save(model);
            unitOfWork.Commit();

            Assert.True(model.Id != default(Guid));
        }

        [Fact]
        public void Should_Delete_Entity_By_Id_NotAsProxy()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            repository.Save(model);
            unitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            repository.Delete<TestItemModel>(model.Id, model.Version, false);
            unitOfWork.Commit();

            var exists = repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Delete_Entity_By_Id_AsProxy()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            repository.Save(model);
            unitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            repository.Delete<TestItemModel>(model.Id, model.Version, true);
            unitOfWork.Commit();

            var exists = repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Delete_Entity()
        {
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();

            repository.Save(model);
            unitOfWork.Commit();

            Assert.True(model.Id != default(Guid));

            repository.Delete(model);
            unitOfWork.Commit();

            var exists = repository.Any<TestItemModel>(q => q.Id == model.Id);
            Assert.False(exists);
        }

        [Fact]
        public void Should_Attach_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(model);
            unitOfWork.Commit();

            var version = model.Version;

            // Load detached version, touch multiple times
            var detachedModel = repository.First<TestItemModel>(model.Id);
            repository.Detach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork.Commit();
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork.Commit();

            Assert.Equal(detachedModel.Version, version);

            // Attach and save again
            repository.Attach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork.Commit();

            Assert.NotEqual(detachedModel.Version, version);
        }

        [Fact]
        public void Should_Detach_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(model);
            unitOfWork.Commit();

            // Touch entity - changes should be saved on flush
            var version = model.Version;

            var attachedModel = repository.First<TestItemModel>(model.Id);
            attachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork.Commit();

            Assert.True(attachedModel.Version > version);
            version = attachedModel.Version;

            // Detach and touch again - changes shouldn't saved on flush
            var detachedModel = repository.First<TestItemModel>(model.Id);
            repository.Detach(detachedModel);
            detachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork.Commit();

            Assert.Equal(detachedModel.Version, version);
        }

        [Fact]
        public void Should_Refresh_Entity()
        {
            // Create entity
            var model = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(model);
            unitOfWork.Commit();

            var version = model.Version;

            // Load attached and detached version, touch multiple times
            var detachedModel = repository.First<TestItemModel>(model.Id);

            // Open another session
            var provider = fixture.Services.BuildServiceProvider();
            //var configuration = provider.GetService<IOptions<DefaultConfigurationSection>>().Options;
            //configuration.Database.ConnectionString = fixture.ConnectionString;
            var repository2 = provider.GetService<IRepository>();
            var unitOfWork2 = provider.GetService<IUnitOfWork>();

            var attachedModel = repository2.First<TestItemModel>(model.Id);
            attachedModel.Name = fixture.TestDataProvider.ProvideRandomString();
            unitOfWork2.Commit();

            Assert.Equal(detachedModel.Version, version);

            // Refresh detached entity - version should be updated
            repository.Refresh(detachedModel);
            Assert.NotEqual(detachedModel.Version, version);
        }
    }
}
