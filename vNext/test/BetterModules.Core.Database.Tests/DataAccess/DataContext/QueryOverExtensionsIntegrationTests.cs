﻿using System;
using System.Linq;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    public class QueryOverExtensionsIntegrationTests : DatabaseTestBase
    {
        private TestItemCategory category1;
        private TestItemModel model1;
        private TestItemModel model2;
        private TestItemModel model3;
        private bool isSet;

        public QueryOverExtensionsIntegrationTests(IServiceCollection services)
        {
            if (!isSet)
            {
                isSet = true;

                category1 = DatabaseTestDataProvider.ProvideRandomTestItemCategory();

                model1 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model1.Name = "QVO_01";
                model2 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model2.Name = "QVO_02";
                model3 = DatabaseTestDataProvider.ProvideRandomTestItemModel(category1);
                model3.Name = "QVO_03";

                Repository.Save(model3);
                Repository.Save(model2);
                Repository.Save(model1);
                
                UnitOfWork.Commit();
            }
        }

        [Fact]
        public void Should_Return_First_Element_Successfully()
        {
            var query = Repository.AsQueryOver<TestItemModel>().Where(t => t.Id == model1.Id);
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
                Repository
                    .AsQueryOver<TestItemModel>()
                    .Where(t => t.Id == guid)
                    .First<TestItemModel, TestItemModel>();
            });
        }

        [Fact]
        public void Should_Add_Default_Paging()
        {
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
            var list = Repository
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
