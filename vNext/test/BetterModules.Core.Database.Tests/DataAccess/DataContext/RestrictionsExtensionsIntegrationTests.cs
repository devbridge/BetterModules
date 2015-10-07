using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using NHibernate.Criterion;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [Collection("Database test collection")]
    public class RestrictionsExtensionsIntegrationTests
    {
        private DatabaseTestFixture fixture;

        public RestrictionsExtensionsIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Filter_Null_Or_Whitespace_Column_Correctly()
        {
            var category = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();
            category.Name = "   ";

            fixture.Repository.Save(category);
            fixture.UnitOfWork.Commit();

            TestItemCategory alias = null;
            var loadedCategory = fixture.Repository
                .AsQueryOver(() => alias)
                .Where(RestrictionsExtensions.IsNullOrWhiteSpace(Projections.Property(() => alias.Name)))
                .And(() => alias.Id == category.Id)
                .SingleOrDefault<TestItemCategory>();

            Assert.NotNull(loadedCategory);
            Assert.Equal(category.Id, loadedCategory.Id);
        }
    }
}
