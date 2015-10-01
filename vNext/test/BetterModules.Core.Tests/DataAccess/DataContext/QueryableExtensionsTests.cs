using System.Collections.Generic;
using System.Linq;

using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using Xunit;

namespace BetterModules.Core.Tests.DataAccess.DataContext
{
    public class QueryableExtensionsTests
    {
        [Fact]
        public void Enumerable_FirstOne_Should_Throw_EntityNotFoundException()
        {
            Assert.Throws<EntityNotFoundException>(delegate
            {
                var list = new List<string>();
                list.AsEnumerable().FirstOne();
            });
        }
        
        [Fact]
        public void Enumerable_FirstOne_Should_Return_First_Item()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var first = list.AsEnumerable().FirstOne();

            Assert.Equal(first, "First");
        }
        
        [Fact]
        public void Queryable_FirstOne_Should_Throw_EntityNotFoundException()
        {
            Assert.Throws<EntityNotFoundException>(delegate
            {
                var list = new List<string>();
                list.AsQueryable().FirstOne();
            });
        }
        
        [Fact]
        public void Queryable_FirstOne_Should_Return_First_Item()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var first = list.AsQueryable().FirstOne();

            Assert.Equal(first, "First");
        }

        [Fact]
        public void Should_Add_Default_Paging()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging().ToList();

            Assert.Equal(list1.Count, 3);
            Assert.Equal("First", list1[0]);
            Assert.Equal("Second", list1[1]);
            Assert.Equal("Third", list1[2]);
        }
        
        [Fact]
        public void Should_Skip_Correct_Count_of_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging(2, 50).ToList();

            Assert.Equal(list1.Count, 2);
            Assert.Equal("Second", list1[0]);
            Assert.Equal("Third", list1[1]);
        }
        
        [Fact]
        public void Should_Skip_Ant_Take_Correct_Count_of_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging(2, 1).ToList();

            Assert.Equal(list1.Count, 1);
            Assert.Equal("Second", list1[0]);
        }
        
        [Fact]
        public void Should_Add_Default_Order_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddOrder().ToList();

            Assert.Equal(list1.Count, 3);
            Assert.Equal("First", list1[0]);
            Assert.Equal("Second", list1[1]);
            Assert.Equal("Third", list1[2]);
        }
        
        [Fact]
        public void Should_Add_Ascending_Order_Items()
        {
            var list = new List<TestModel> { new TestModel("02"), new TestModel("03"), new TestModel("01") };
            var list1 = list.AsQueryable().AddOrder(t => t.Name).ToList();

            Assert.Equal(list1.Count, 3);
            Assert.Equal("01", list1[0].Name);
            Assert.Equal("02", list1[1].Name);
            Assert.Equal("03", list1[2].Name);
        }
        
        [Fact]
        public void Should_Add_Descending_Order_Items()
        {
            var list = new List<TestModel> { new TestModel("02"), new TestModel("03"), new TestModel("01") };
            var list1 = list.AsQueryable().AddOrder(t => t.Name, true).ToList();

            Assert.Equal(list1.Count, 3);
            Assert.Equal("03", list1[0].Name);
            Assert.Equal("02", list1[1].Name);
            Assert.Equal("01", list1[2].Name);
        }
        
        private class TestModel
        {
            public TestModel(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
        }
    }
}
