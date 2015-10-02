using System.Security.Principal;
using Autofac;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Web.Dependencies;
using BetterModules.Core.Web.Mvc;
using BetterModules.Core.Web.Mvc.Commands;
using BetterModules.Core.Web.Tests.TestHelpers;
using BetterModules.Core.Web.Web;
using Moq;
using NUnit.Framework;

namespace BetterModules.Core.Web.Tests.Mvc.Commands
{
    [TestFixture]
    public class DefaultCommandResolverTests : TestBase
    {
        [Test]
        public void ShouldResolve_Commands_Successfully()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType(typeof(CommandTest)).AsSelf();
            ContextScopeProvider.RegisterTypes(containerBuilder);

            var httpContextMoq = new HttpContextMoq();
            var accessor = new Mock<IHttpContextAccessor>();
            accessor
                .Setup(a => a.GetCurrent())
                .Returns(() => httpContextMoq.HttpContextBase);

            var provider = new PerWebRequestContainerProvider(accessor.Object);
            var resolver = new DefaultCommandResolver(provider);

            var commandContext = new CommandContextTest();
            var command = resolver.ResolveCommand<CommandTest>(commandContext);
            Assert.IsNotNull(command);
            Assert.AreEqual(command.Context, commandContext);
        }

        private class CommandTest : ICommand
        {
            public void Execute()
            {
            }

            public ICommandContext Context { get; set; }
        }

        private class CommandContextTest : ICommandContext
        {
            public IMessagesIndicator Messages { get; set; }
            public IPrincipal Principal { get; set; }
        }
    }
}
