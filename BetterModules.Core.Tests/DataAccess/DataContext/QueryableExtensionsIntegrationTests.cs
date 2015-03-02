using System.Collections.Generic;
using System.Linq;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using NHibernate.Linq;
using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class QueryableExtensionsIntegrationTests : DatabaseTestBase
    {
        [Test]
        public void Should_Return_Correct_Items_FutureWrapper_Count()
        {
            var list = new List<string> { "First", "Second", "Third" }.AsQueryable();
            var count = list.ToRowCountFutureValue().Value;

            Assert.AreEqual(count, 3);
        }

        [Test]
        public void Should_Return_Correct_Items_Future_Count()
        {
            var category1 = TestDataProvider.ProvideRandomTestItemCategory();
            var category2 = TestDataProvider.ProvideRandomTestItemCategory();
            var category3 = TestDataProvider.ProvideRandomTestItemCategory();
            category1.Name = "QEIT_" + category1.Name.Substring(10);
            category2.Name = "QEIT_" + category1.Name.Substring(10);
            category3.Name = "QEIT_" + category1.Name.Substring(10);

            Repository.Save(category1);
            Repository.Save(category2);
            Repository.Save(category3);
            UnitOfWork.Commit();

            var query = Repository.AsQueryable<TestItemCategory>().Where(c => c.Name.StartsWith("QEIT_"));
            var countFuture = query.ToRowCountFutureValue();
            var future = query.ToFuture();

            Assert.AreEqual(future.ToList().Count, 3);
            Assert.AreEqual(countFuture.Value, 3);
        }
    }
}
