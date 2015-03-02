using BetterModules.Sample.Module.Models;

namespace BetterModules.Core.Database.Tests.TestHelpers
{
    public class DatabaseRandomTestDataProvider : Core.Tests.TestHelpers.RandomTestDataProvider
    {
        public TestItemModel ProvideRandomTestItemModel(TestItemCategory category = null)
        {
            var model = new TestItemModel();
            model.Name = ProvideRandomString();

            model.Category = category ?? ProvideRandomTestItemCategory();

            return model;
        }

        public TestItemCategory ProvideRandomTestItemCategory()
        {
            var model = new TestItemCategory();
            model.Name = ProvideRandomString();
            
            return model;
        }
    }
}
