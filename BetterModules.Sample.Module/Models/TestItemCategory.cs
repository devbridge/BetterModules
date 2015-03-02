using BetterModules.Core.Models;

namespace BetterModules.Sample.Module.Models
{
    public class TestItemCategory : EquatableEntity<TestItemCategory>
    {
        public virtual string Name { get; set; }
    }
}
