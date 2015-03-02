using BetterModules.Core.Models;

namespace BetterModules.Sample.Module.Models
{
    public class TestItemModelChild : EquatableEntity<TestItemModelChild>
    {
        public virtual string Name { get; set; }

        public virtual TestItemCategory Category { get; set; }
        
        public virtual TestItemModel Item { get; set; }
    }
}
