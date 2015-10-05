using System.Linq;
using Autofac;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.Dependencies;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Fetching
{
    [TestFixture]
    public class DefaultFetchingProviderIntegrationTests : DatabaseTestBase
    {
        [Test]
        public void Should_Call_Fetch_Successfully()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var repository = container.Resolve<IRepository>();

                var query = repository.AsQueryable<TestItemModel>().Where(q => q.Name == "Item1");
                query = new DefaultFetchingProvider().Fetch(query, q => q.Category);
                var result = query.ToList();

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Count, 1);
                Assert.IsNotNull(result[0].Category);
                Assert.AreEqual(result[0].Category.Name, "ItemCategory1");
            }
        }
        
        [Test]
        public void Should_Call_FetchMany_Successfully()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var repository = container.Resolve<IRepository>();

                var query = repository.AsQueryable<TestItemModel>().Where(q => q.Name == "Item1");
                query = new DefaultFetchingProvider().FetchMany(query, q => q.Children);
                var result = query.ToList();

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Count, 1);
                Assert.IsNotNull(result[0].Children);
                Assert.AreEqual(result[0].Children.Count, 2);
                Assert.IsTrue(result[0].Children.Any(c => c.Name == "Item1 Child1"));
                Assert.IsTrue(result[0].Children.Any(c => c.Name == "Item1 Child2"));
            }
        }
        
        [Test]
        public void Should_Call_ThenFetch_Successfully()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var repository = container.Resolve<IRepository>();

                var query = repository.AsQueryable<TestItemModelChild>().Where(q => q.Name == "Item1 Child1");
                
                IFetchRequest<TestItemModelChild, TestItemModel> query1 = new DefaultFetchingProvider().Fetch(query, q => q.Item);
                query = new DefaultFetchingProvider().ThenFetch(query1, q => q.Category);
                
                var result = query.ToList();

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Count, 1);
                Assert.IsNotNull(result[0].Item);
                Assert.AreEqual(result[0].Item.Name, "Item1");
                Assert.IsNotNull(result[0].Item.Category);
                Assert.AreEqual(result[0].Item.Category.Name, "ItemCategory1");
            }
        }
        
        [Test]
        public void Should_Call_ThenFetchMany_Successfully()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var repository = container.Resolve<IRepository>();

                var query = repository.AsQueryable<TestItemModelChild>().Where(q => q.Name == "Item1 Child1");
                
                IFetchRequest<TestItemModelChild, TestItemModel> query1 = new DefaultFetchingProvider().Fetch(query, q => q.Item);
                query = new DefaultFetchingProvider().ThenFetchMany(query1, q => q.Children);
                
                var result = query.ToList();

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Count, 1);
                Assert.IsNotNull(result[0].Item);
                Assert.AreEqual(result[0].Item.Name, "Item1");
                Assert.IsNotNull(result[0].Item.Children);
                Assert.AreEqual(result[0].Item.Children.Count, 2);
                Assert.IsTrue(result[0].Item.Children.Any(c => c.Name == "Item1 Child1"));
                Assert.IsTrue(result[0].Item.Children.Any(c => c.Name == "Item1 Child2"));
            }
        }
    }
}
