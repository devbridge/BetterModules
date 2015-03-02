using System;
using BetterModules.Sample.Module;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Models
{
    [TestFixture]
    public class EntityMapBaseIntegrationTests : DatabaseTestBase
    {
        [Test]
        public void Should_Load_And_Map_BaseEntity_Correctly()
        {
            var category = Repository.FirstOrDefault<TestItemCategory>(c => c.Name == "ItemCategory1");
            var item = Repository.FirstOrDefault<TestItemModel>(SampleModuleDescriptor.TestItemModelId);

            // Base properties
            Assert.AreEqual(item.Id, SampleModuleDescriptor.TestItemModelId);
            Assert.AreEqual(item.Version, 1);
            Assert.AreEqual(item.IsDeleted, false);
            Assert.AreEqual(item.DeletedOn, null);
            Assert.AreEqual(item.DeletedByUser, null);
            Assert.AreEqual(item.CreatedByUser, "Creator");
            Assert.AreEqual(item.ModifiedByUser, "Modifier");
            Assert.AreEqual(item.CreatedOn, new DateTime(2015, 1, 1, 1, 1, 1));
            Assert.AreEqual(item.ModifiedOn, new DateTime(2015, 2, 2, 2, 2, 2));

            // Item properties
            Assert.AreEqual(item.Name, "Item1");
            Assert.AreEqual(item.Category.Id, category.Id);
        }
    }
}