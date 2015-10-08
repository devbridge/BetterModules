using System;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using Microsoft.Framework.DependencyInjection;
using BetterModules.Core.Security;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    [Collection("Database test collection")]
    public class DeleteEventListenerIntegrationTests
    {
        private DatabaseTestFixture fixture;
        private readonly IRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteEventListenerIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
            repository = fixture.Provider.GetService<IRepository>();
            unitOfWork = fixture.Provider.GetService<IUnitOfWork>();
        }

        [Fact]
        public void Should_Mark_Entity_As_Deleted()
        {
            var entity = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel();
            repository.Save(entity);
            unitOfWork.Commit();

            Assert.NotSame(entity.Id.ToString(), Guid.Empty.ToString());
            repository.Delete(entity);
            unitOfWork.Commit();

            var principalProvider = fixture.Provider.GetService<IPrincipalProvider>();

            Assert.True(entity.IsDeleted);
            Assert.NotNull(entity.DeletedOn);
            Assert.Equal(entity.DeletedByUser, principalProvider.CurrentPrincipalName);
        }
    }
}
