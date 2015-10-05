using System;
using Microsoft.Framework.DependencyInjection;
using BetterModules.Core.Security;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.EventListeners
{
    public class DeleteEventListenerIntegrationTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Mark_Entity_As_Deleted()
        {
            var entity = DatabaseTestDataProvider.ProvideRandomTestItemModel();
            Repository.Save(entity);
            UnitOfWork.Commit();

            Assert.NotSame(entity.Id.ToString(), Guid.Empty.ToString());
            Repository.Delete(entity);
            UnitOfWork.Commit();

            var principalProvider = Provider.GetService<IPrincipalProvider>();

            Assert.True(entity.IsDeleted);
            Assert.NotNull(entity.DeletedOn);
            Assert.Equal(entity.DeletedByUser, principalProvider.CurrentPrincipalName);
        }
    }
}
