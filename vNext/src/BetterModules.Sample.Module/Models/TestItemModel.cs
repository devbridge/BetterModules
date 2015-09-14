using System.Collections.Generic;
using BetterModules.Core.Models;

namespace BetterModules.Sample.Module.Models
{
    public class TestItemModel : EquatableEntity<TestItemModel>
    {
        public virtual string Name { get; set; }

        public virtual TestItemCategory Category { get; set; }

        public virtual IList<TestItemModelChild> Children { get; set; }
    }
}
