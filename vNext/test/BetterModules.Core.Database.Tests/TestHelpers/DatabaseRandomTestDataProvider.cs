using BetterModules.Sample.Module.Models;

namespace BetterModules.Core.Database.Tests.TestHelpers
{
    public class DatabaseRandomTestDataProvider : Core.Tests.TestHelpers.RandomTestDataProvider
    {
        public TestItemModel ProvideRandomTestItemModel(TestItemCategory category = null)
        {
            var model = new TestItemModel
            {
                Name = ProvideRandomString(),
                Category = category ?? ProvideRandomTestItemCategory()
            };


            return model;
        }

        public TestItemCategory ProvideRandomTestItemCategory()
        {
            var model = new TestItemCategory {Name = ProvideRandomString()};

            return model;
        }
    }
}
