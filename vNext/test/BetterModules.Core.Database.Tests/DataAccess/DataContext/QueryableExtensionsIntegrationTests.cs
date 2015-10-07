using System.Collections.Generic;
using System.Linq;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using NHibernate.Linq;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [Collection("Database test collection")]
    public class QueryableExtensionsIntegrationTests
    {
        private DatabaseTestFixture fixture;

        public QueryableExtensionsIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Return_Correct_Items_FutureWrapper_Count()
        {
            var list = new List<string> { "First", "Second", "Third" }.AsQueryable();
            var count = list.ToRowCountFutureValue().Value;

            Assert.Equal(count, 3);
        }

        [Fact]
        public void Should_Return_Correct_Items_Future_Count()
        {
            var category1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();
            var category2 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();
            var category3 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();
            category1.Name = "QEIT_" + category1.Name.Substring(10);
            category2.Name = "QEIT_" + category1.Name.Substring(10);
            category3.Name = "QEIT_" + category1.Name.Substring(10);

            fixture.Repository.Save(category1);
            fixture.Repository.Save(category2);
            fixture.Repository.Save(category3);
            fixture.UnitOfWork.Commit();

            var query = fixture.Repository.AsQueryable<TestItemCategory>().Where(c => c.Name.StartsWith("QEIT_"));
            var countFuture = query.ToRowCountFutureValue();
            var future = query.ToFuture();

            Assert.Equal(future.ToList().Count, 3);
            Assert.Equal(countFuture.Value, 3);
        }
    }
}
