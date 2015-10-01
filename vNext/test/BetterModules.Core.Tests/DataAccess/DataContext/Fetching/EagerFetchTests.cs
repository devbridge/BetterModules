//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//using BetterModules.Core.DataAccess.DataContext.Fetching;
//using BetterModules.Core.Tests.TestHelpers;

//using Moq;

//using NUnit.Framework;
//using Xunit;

//namespace BetterModules.Core.Tests.DataAccess.DataContext.Fetching
//{
//    [TestFixture]
//    public class EagerFetchTests : TestBase
//    {
//        private static IQueryable<TestModel> originalQuery = new List<TestModel>
//        {
//            new TestModel {Name = "Test1"},
//            new TestModel {Name = "Test2"},
//        }.AsQueryable();

//        [Fact]
//        public void Should_Call_Fetch_Successfully()
//        {
//            using (var fakeProvider = new ContextScopeProviderHelper())
//            {
//                var provider = GetProviderMock();
//                fakeProvider.RegisterFakeServiceInstance(provider, typeof(IFetchingProvider));

//                var result = originalQuery.Fetch(s => s.Parent);

//                Assert.NotNull(result);
//                Assert.Equal(result.Count(), 1);
//                Assert.True(result.Any(r => r.Name == "Fetch_Result"));
//            }
//        }
        
//        [Fact]
//        public void Should_Call_FetchMany_Successfully()
//        {
//            using (var fakeProvider = new ContextScopeProviderHelper())
//            {
//                var provider = GetProviderMock();
//                fakeProvider.RegisterFakeServiceInstance(provider, typeof(IFetchingProvider));

//                var result = originalQuery.FetchMany(s => s.Children);

//                Assert.NotNull(result);
//                Assert.Equal(result.Count(), 1);
//                Assert.True(result.Any(r => r.Name == "FetchMany_Result"));
//            }
//        }
        
//        [Fact]
//        public void Should_Call_ThenFetch_Successfully()
//        {
//            using (var fakeProvider = new ContextScopeProviderHelper())
//            {
//                var provider = GetProviderMock();
//                fakeProvider.RegisterFakeServiceInstance(provider, typeof(IFetchingProvider));

//                var result = originalQuery.Fetch(s => s.Parent).ThenFetch(s => s.Parent);

//                Assert.NotNull(result);
//                Assert.Equal(result.Count(), 1);
//                Assert.True(result.Any(r => r.Name == "ThenFetch_Result"));
//            }
//        }
        
//        [Fact]
//        public void Should_Call_ThenFetchMany_Successfully()
//        {
//            using (var fakeProvider = new ContextScopeProviderHelper())
//            {
//                var provider = GetProviderMock();
//                fakeProvider.RegisterFakeServiceInstance(provider, typeof(IFetchingProvider));

//                var result = originalQuery.FetchMany(s => s.Children).ThenFetchMany(s => s.Children);

//                Assert.NotNull(result);
//                Assert.Equal(result.Count(), 1);
//                Assert.True(result.Any(r => r.Name == "ThenFetchMany_Result"));
//            }
//        }

//        private IFetchingProvider GetProviderMock()
//        {
//            var provider = new Mock<IFetchingProvider>();
//            provider
//                .Setup(p => p.Fetch(It.IsAny<IQueryable<TestModel>>(), It.IsAny<Expression<Func<TestModel, TestModel>>>()))
//                .Returns<IQueryable<TestModel>, Expression<Func<TestModel, TestModel>>>((query, expression) =>
//                {
//                    Assert.Equal(query, originalQuery);

//                    var result = new List<TestModel> {new TestModel {Name = "Fetch_Result"}}.AsQueryable();
//                    return new FetchRequestTest<TestModel, TestModel>(result);
//                });
            
//            provider
//                .Setup(p => p.ThenFetch(It.IsAny<FetchRequestTest<TestModel, TestModel>>(), It.IsAny<Expression<Func<TestModel, TestModel>>>()))
//                .Returns<FetchRequestTest<TestModel, TestModel>, Expression<Func<TestModel, TestModel>>>((query, expression) =>
//                {
//                    Assert.NotNull(query);
//                    Assert.Equal(query.Count(), 1);
//                    Assert.True(query.Any(r => r.Name == "Fetch_Result"));

//                    var result = new List<TestModel> {new TestModel {Name = "ThenFetch_Result"}}.AsQueryable();
//                    return new FetchRequestTest<TestModel, TestModel>(result);
//                });
            
//            provider
//                .Setup(p => p.FetchMany(It.IsAny<IQueryable<TestModel>>(), It.IsAny<Expression<Func<TestModel, IEnumerable<TestModel>>>>()))
//                .Returns<IQueryable<TestModel>, Expression<Func<TestModel, IEnumerable<TestModel>>>>((query, expression) =>
//                {
//                    Assert.Equal(query, originalQuery);

//                    var result = new List<TestModel> {new TestModel {Name = "FetchMany_Result"}}.AsQueryable();
//                    return new FetchRequestTest<TestModel, TestModel>(result);
//                });
            
//            provider
//                .Setup(p => p.ThenFetchMany(It.IsAny<FetchRequestTest<TestModel, TestModel>>(), It.IsAny<Expression<Func<TestModel, IEnumerable<TestModel>>>>()))
//                .Returns<FetchRequestTest<TestModel, TestModel>, Expression<Func<TestModel, IEnumerable<TestModel>>>>((query, expression) =>
//                {
//                    Assert.NotNull(query);
//                    Assert.Equal(query.Count(), 1);
//                    Assert.True(query.Any(r => r.Name == "FetchMany_Result"));

//                    var result = new List<TestModel> {new TestModel {Name = "ThenFetchMany_Result"}}.AsQueryable();
//                    return new FetchRequestTest<TestModel, TestModel>(result);
//                });

//            return provider.Object;
//        }

//        private class TestModel
//        {
//            public string Name { get; set; }

//            public TestModel Parent { get; set; }
            
//            public IEnumerable<TestModel> Children { get; set; }
//        }

//        private class FetchRequestTest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
//        {
//            public readonly IQueryable<TQueried> query;

//            public IEnumerator<TQueried> GetEnumerator()
//            {
//                return query.GetEnumerator();
//            }

//            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//            {
//                return query.GetEnumerator();
//            }

//            public Type ElementType
//            {
//                get { return query.ElementType; }
//            }

//            public Expression Expression
//            {
//                get { return query.Expression; }
//            }

//            public IQueryProvider Provider
//            {
//                get { return query.Provider; }
//            }

//            public FetchRequestTest(IQueryable<TQueried> query)
//            {
//                this.query = query;
//            }
//        }
//    }
//}
