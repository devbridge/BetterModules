using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using NHibernate.Criterion;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class RestrictionsExtensionsIntegrationTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Filter_Null_Or_Whitespace_Column_Correctly()
        {
            var category = DatabaseTestDataProvider.ProvideRandomTestItemCategory();
            category.Name = "   ";

            Repository.Save(category);
            UnitOfWork.Commit();

            TestItemCategory alias = null;
            var loadedCategory = Repository
                .AsQueryOver(() => alias)
                .Where(RestrictionsExtensions.IsNullOrWhiteSpace(Projections.Property(() => alias.Name)))
                .And(() => alias.Id == category.Id)
                .SingleOrDefault<TestItemCategory>();

            Assert.IsNotNull(loadedCategory);
            Assert.Equal(category.Id, loadedCategory.Id);
        }
    }
}
