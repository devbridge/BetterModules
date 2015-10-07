﻿using BetterModules.Core.DataAccess.DataContext;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext
{
    [Collection("Database test collection")]
    public class DefaultUnitOfWorkFactoryIntegrationTests
    {
        private DatabaseTestFixture fixture;

        public DefaultUnitOfWorkFactoryIntegrationTests(DatabaseTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Create_New_Unit_Of_Work()
        {
            var factory = new DefaultUnitOfWorkFactory(fixture.Provider.GetService<ISessionFactoryProvider>());
            var uow = factory.New();

            Assert.NotNull(uow);
            Assert.NotEqual(uow, fixture.UnitOfWork);
        }
    }
}
