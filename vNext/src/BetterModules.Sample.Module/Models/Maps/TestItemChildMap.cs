using BetterModules.Core.Models;
using BetterModules.Core.Modules.Registration;

namespace BetterModules.Sample.Module.Models.Maps
{
    public class TestItemChildMap : EntityMapBase<TestItemModelChild>
    {
        public TestItemChildMap(IModulesRegistration modulesRegistration)
            : base(SampleModuleDescriptor.ModuleName, modulesRegistration)
        {
            Table("TestItemChildren");

            Map(x => x.Name).Not.Nullable().Length(100);
            References(x => x.Category).Column("TestItemCategoryId").Cascade.SaveUpdate().LazyLoad();
            References(x => x.Item).Column("TestItemId").Cascade.SaveUpdate().LazyLoad();
        }
    }
}
