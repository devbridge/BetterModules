using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using NHibernate.Criterion;
using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class RestrictionsExtensionsIntegrationTests : DatabaseTestBase
    {
        [Test]
        public void Should_Filter_Null_Or_Whitespace_Column_Correctly()
        {
            var category = TestDataProvider.ProvideRandomTestItemCategory();
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
            Assert.AreEqual(category.Id, loadedCategory.Id);
        }
    }
}
