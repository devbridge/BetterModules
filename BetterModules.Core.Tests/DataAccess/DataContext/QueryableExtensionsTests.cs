using System.Collections.Generic;
using System.Linq;

using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;

using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext
{
    [TestFixture]
    public class QueryableExtensionsTests
    {
        [Test]
        public void Enumerable_FirstOne_Should_Throw_EntityNotFoundException()
        {
            var list = new List<string>();
            Assert.Throws<EntityNotFoundException>(() =>
            {
                list.AsEnumerable().FirstOne();
            });
        }

        [Test]
        public void Enumerable_FirstOne_Should_Return_First_Item()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var first = list.AsEnumerable().FirstOne();

            Assert.AreEqual(first, "First");
        }
        
        [Test]
        public void Queryable_FirstOne_Should_Throw_EntityNotFoundException()
        {
            var list = new List<string>();
            Assert.Throws<EntityNotFoundException>(() =>
            {
                list.AsQueryable().FirstOne();
            });
        }
        
        [Test]
        public void Queryable_FirstOne_Should_Return_First_Item()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var first = list.AsQueryable().FirstOne();

            Assert.AreEqual(first, "First");
        }

        [Test]
        public void Should_Add_Default_Paging()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging().ToList();

            Assert.AreEqual(list1.Count, 3);
            Assert.AreEqual("First", list1[0]);
            Assert.AreEqual("Second", list1[1]);
            Assert.AreEqual("Third", list1[2]);
        }
        
        [Test]
        public void Should_Skip_Correct_Count_of_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging(2, 50).ToList();

            Assert.AreEqual(list1.Count, 2);
            Assert.AreEqual("Second", list1[0]);
            Assert.AreEqual("Third", list1[1]);
        }
        
        [Test]
        public void Should_Skip_Ant_Take_Correct_Count_of_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddPaging(2, 1).ToList();

            Assert.AreEqual(list1.Count, 1);
            Assert.AreEqual("Second", list1[0]);
        }
        
        [Test]
        public void Should_Add_Default_Order_Items()
        {
            var list = new List<string> { "First", "Second", "Third" };
            var list1 = list.AsQueryable().AddOrder().ToList();

            Assert.AreEqual(list1.Count, 3);
            Assert.AreEqual("First", list1[0]);
            Assert.AreEqual("Second", list1[1]);
            Assert.AreEqual("Third", list1[2]);
        }
        
        [Test]
        public void Should_Add_Ascending_Order_Items()
        {
            var list = new List<TestModel> { new TestModel("02"), new TestModel("03"), new TestModel("01") };
            var list1 = list.AsQueryable().AddOrder(t => t.Name).ToList();

            Assert.AreEqual(list1.Count, 3);
            Assert.AreEqual("01", list1[0].Name);
            Assert.AreEqual("02", list1[1].Name);
            Assert.AreEqual("03", list1[2].Name);
        }
        
        [Test]
        public void Should_Add_Descending_Order_Items()
        {
            var list = new List<TestModel> { new TestModel("02"), new TestModel("03"), new TestModel("01") };
            var list1 = list.AsQueryable().AddOrder(t => t.Name, true).ToList();

            Assert.AreEqual(list1.Count, 3);
            Assert.AreEqual("03", list1[0].Name);
            Assert.AreEqual("02", list1[1].Name);
            Assert.AreEqual("01", list1[2].Name);
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
