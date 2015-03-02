using BetterModules.Core.Models;

namespace BetterModules.Sample.Module.Models.Maps
{
    public class TestItemCategoryMap : EntityMapBase<TestItemCategory>
    {
        public TestItemCategoryMap()
            : base(SampleModuleDescriptor.ModuleName)
        {
            Table("TestItemCategories");
            
            Map(x => x.Name).Not.Nullable().Length(100);
        }
    }
}
