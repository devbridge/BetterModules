using System;
using BetterModules.Core.DataAccess;
using BetterModules.Sample.Module;
using BetterModules.Sample.Module.Models;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.Models
{
    [Collection("Database test collection")]
    public class EntitySubClassMapBaseIntergrationTests
    {
        private readonly IRepository repository;

        public EntitySubClassMapBaseIntergrationTests(DatabaseTestFixture fixture)
        {
            var provider = fixture.Services.BuildServiceProvider();
            repository = provider.GetService<IRepository>();
        }

        [Fact]
        public void Should_Load_And_Map_BaseEntity_Correctly()
        {
            var category = repository.FirstOrDefault<TestItemCategory>(c => c.Name == "ItemCategory1");
            var item = repository.FirstOrDefault<InheritedTestItemModel>(SampleModuleDescriptor.TestItemModelId);

            Assert.NotNull(item);
            Assert.NotNull(category);

            // Base properties
            Assert.Equal(item.Id, SampleModuleDescriptor.TestItemModelId);
            Assert.Equal(item.Version, 1);
            Assert.Equal(item.IsDeleted, false);
            Assert.Equal(item.DeletedOn, null);
            Assert.Equal(item.DeletedByUser, null);
            Assert.Equal(item.CreatedByUser, "Creator");
            Assert.Equal(item.ModifiedByUser, "Modifier");
            Assert.Equal(item.CreatedOn, new DateTime(2015, 1, 1, 1, 1, 1));
            Assert.Equal(item.ModifiedOn, new DateTime(2015, 2, 2, 2, 2, 2));

            // Item properties
            Assert.Equal(item.Name, "Item1");
            Assert.Equal(item.Category.Id, category.Id);

            // Inherited Item properties
            Assert.Equal(item.Description, "Inherited Item1");
        }
    }
}
