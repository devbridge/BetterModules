using System;
using System.Linq;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [Collection("Database test collection")]
    public class QueryOverExtensionsIntegrationTests
    {
        private DatabaseTestFixture fixture;
        private TestItemCategory category1;
        private TestItemModel model1;
        private TestItemModel model2;
        private TestItemModel model3;
        private bool isSet;

        public QueryOverExtensionsIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
            if (!isSet)
            {
                isSet = true;

                category1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemCategory();

                model1 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model1.Name = "QVO_01";
                model2 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model2.Name = "QVO_02";
                model3 = fixture.DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model3.Name = "QVO_03";

                fixture.Repository.Save(model3);
                fixture.Repository.Save(model2);
                fixture.Repository.Save(model1);

                fixture.UnitOfWork.Commit();
            }
        }

        [Fact]
        public void Should_Return_First_Element_Successfully()
        {
            var query = fixture.Repository.AsQueryOver<TestItemModel>().Where(t => t.Id == model1.Id);
            var item = query.First<TestItemModel, TestItemModel>();

            Assert.NotNull(item);
            Assert.Equal(item.Name, model1.Name);
        }
        
        [Fact]
        public void Should_Throw_EntityNotFound_Exception_Retrieving_First()
        {
            Assert.Throws<EntityNotFoundException>(() =>
            {
                var guid = Guid.NewGuid();
                fixture.Repository
                    .AsQueryOver<TestItemModel>()
                    .Where(t => t.Id == guid)
                    .First<TestItemModel, TestItemModel>();
            });
        }

        [Fact]
        public void Should_Add_Default_Paging()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .Where(t => t.Category == category1)
                .AddPaging(null, null)
                .OrderBy(t => t.Name).Asc
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 3);
            Assert.Equal(model1.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
            Assert.Equal(model3.Name, list[2].Name);
        }

        [Fact]
        public void Should_Return_First_Page_Of_Items()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .Where(t => t.Category == category1)
                .AddPaging(1, 2)
                .OrderBy(t => t.Name).Asc
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 2);
            Assert.Equal(model1.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
        }

        [Fact]
        public void Should_Return_Second_Page_Of_Items()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .Where(t => t.Category == category1)
                .AddPaging(2, 2)
                .OrderBy(t => t.Name).Asc
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 1);
            Assert.Equal(model3.Name, list[0].Name);
        }

        [Fact]
        public void Should_Apply_Filter_Order_Asc_Paging_Correctly()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .ApplyFilters(t => t.Category == category1, t => t.Name, false, 1, 2)
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 2);
            Assert.Equal(model1.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
        }
        
        [Fact]
        public void Should_Apply_Filter_Order_Desc_Paging_Correctly()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .ApplyFilters(t => t.Category == category1, t => t.Name, true, 1, 2)
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 2);
            Assert.Equal(model3.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
        }
        
        [Fact]
        public void Should_Apply_Filter_Order_Desc_No_Paging_Correctly()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .ApplyFilters(t => t.Category == category1, t => t.Name, true)
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 3);
            Assert.Equal(model3.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
            Assert.Equal(model1.Name, list[2].Name);
        }

        [Fact]
        public void Should_Apply_SubQuery_Filter_Order_Desc_Paging_Correctly()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .ApplySubQueryFilters(t => t.Category == category1, t => t.Name, true, 1, 2)
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 2);
            Assert.Equal(model3.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
        }
        
        [Fact]
        public void Should_Apply_SubQuery_Filter_Order_Desc_No_Paging_Correctly()
        {
            var list = fixture.Repository
                .AsQueryOver<TestItemModel>()
                .ApplySubQueryFilters(t => t.Category == category1, t => t.Name, true)
                .List<TestItemModel>()
                .ToList();

            Assert.Equal(list.Count, 3);
            Assert.Equal(model3.Name, list[0].Name);
            Assert.Equal(model2.Name, list[1].Name);
            Assert.Equal(model1.Name, list[2].Name);
        }
    }
}
