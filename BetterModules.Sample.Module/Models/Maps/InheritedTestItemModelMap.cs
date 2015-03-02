using BetterModules.Core.Models;

namespace BetterModules.Sample.Module.Models.Maps
{
    public class InheritedTestItemModelMap : EntitySubClassMapBase<InheritedTestItemModel>
    {
        public InheritedTestItemModelMap()
            : base(SampleModuleDescriptor.ModuleName)
        {
            Table("InheritedTestItems");
            
            Map(x => x.Description).Not.Nullable().Length(100);
        }
    }
}
