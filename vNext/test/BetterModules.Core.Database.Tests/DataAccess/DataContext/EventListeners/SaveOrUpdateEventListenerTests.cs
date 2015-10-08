using System;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [Collection("Database test collection")]
    public class SaveOrUpdateEventListenerTests
    {
        private DatabaseTestFixture fixture;
        private readonly IRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public SaveOrUpdateEventListenerTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
            repository = fixture.Provider.GetService<IRepository>();
            unitOfWork = fixture.Provider.GetService<IUnitOfWork>();
        }

        [Fact]
        public void Should_Update_Entity_Properties_When_Creating()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(entity);
            unitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);
        }
        
        [Fact]
        public void Should_Update_Entity_Properties_When_Updating()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(entity);
            unitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.NotNull(entity.CreatedOn);
            Assert.Equal(entity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(entity.ModifiedOn);
            Assert.Equal(entity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            var modified = entity.ModifiedOn;

            var loadedEntity = repository.FirstOrDefault<TestItemModel>(entity.Id);
            loadedEntity.Name = fixture.TestDataProvider.ProvideRandomString(100);
            repository.Save(loadedEntity);
            unitOfWork.Commit();

            Assert.NotNull(loadedEntity.CreatedOn);
            Assert.Equal(loadedEntity.CreatedByUser, principalProvider.CurrentPrincipalName);
            Assert.NotNull(loadedEntity.ModifiedOn);
            Assert.Equal(loadedEntity.ModifiedByUser, principalProvider.CurrentPrincipalName);

            Assert.Equal(loadedEntity.CreatedOn, entity.CreatedOn);
            Assert.NotEqual(loadedEntity.ModifiedOn, modified);
        }
    }
}
