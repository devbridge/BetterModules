using BetterModules.Core.Models;
using BetterModules.Core.Modules.Registration;

namespace BetterModules.Sample.Module.Models.Maps
{
    public class TestItemMap : EntityMapBase<TestItemModel>
    {
        public TestItemMap()
            : base(SampleModuleDescriptor.ModuleName)
        {
            Table("TestItems");
            
            Map(x => x.Name).Not.Nullable().Length(100);
            References(x => x.Category).Column("TestItemCategoryId").Cascade.SaveUpdate().LazyLoad();
            HasMany(f => f.Children).Table("TestItemChildren").KeyColumn("TestItemId").Inverse().Cascade.SaveUpdate().Where("IsDeleted = 0");
        }
    }
}
