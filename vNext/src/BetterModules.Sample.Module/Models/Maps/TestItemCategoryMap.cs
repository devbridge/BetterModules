using BetterModules.Core.Models;
using BetterModules.Core.Modules.Registration;

namespace BetterModules.Sample.Module.Models.Maps
{
    public class TestItemCategoryMap : EntityMapBase<TestItemCategory>
    {
        public TestItemCategoryMap(IModulesRegistration modulesRegistration)
            : base(SampleModuleDescriptor.ModuleName, modulesRegistration)
        {
            Table("TestItemCategories");
            
            Map(x => x.Name).Not.Nullable().Length(100);
        }
    }
}
