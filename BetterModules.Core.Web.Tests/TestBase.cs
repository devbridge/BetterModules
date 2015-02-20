using BetterModules.Core.Web.Tests.TestHelpers;

namespace BetterModules.Core.Web.Tests
{
    public class TestBase
    {
        private RandomTestDataProvider testDataProvider;

        private static bool started;

        protected TestBase()
        {
            if (!started)
            {
                WebApplicationContext.IsTestMode = true;
                WebApplication.Initialize();

                started = true;
            }
        }

        public RandomTestDataProvider TestDataProvider
        {
            get
            {
                if (testDataProvider == null)
                {
                    testDataProvider = new RandomTestDataProvider();
                }
                return testDataProvider;
            }
        }
    }
}
