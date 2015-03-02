using Autofac;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Tests.TestHelpers;
using NUnit.Framework;

namespace BetterModules.Core.Tests
{
    public abstract class TestBase
    {
        private RandomTestDataProvider testDataProvider;

        private static bool started;

        private ILifetimeScope container;

        protected ILifetimeScope Container
        {
            get
            {
                if (container == null)
                {
                    container = ContextScopeProvider.CreateChildContainer();
                }
                return container;
            }
        }

        protected TestBase()
        {
            if (!started)
            {
                Application.Initialize();

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

        [TestFixtureTearDown]
        public void Down()
        {
            OnTextFixtureDown();
        }

        protected virtual void OnTextFixtureDown()
        {
            if (Container != null)
            {
                container.Dispose();
            }
        }
    }
}
