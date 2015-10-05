using System.Security.Principal;
using BetterModules.Core.Infrastructure;
using BetterModules.Core.Infrastructure.Commands;
using BetterModules.Core.Web.Web;
using Microsoft.Framework.DependencyInjection;
using Moq;
using Xunit;

namespace BetterModules.Core.Web.Tests.Mvc.Commands
{
    public class DefaultCommandResolverTests
    {
        // TODO: Fix the test
        //[Fact]
        //public void ShouldResolve_Commands_Successfully()
        //{
        //    var services = new ServiceCollection();
        //    services.AddScoped<CommandTest>();

        //    var httpContextMoq = new HttpContextMoq();
        //    var accessor = new Mock<IHttpContextAccessor>();
        //    accessor
        //        .Setup(a => a.GetCurrent())
        //        .Returns(() => httpContextMoq.HttpContextBase);

        //    var provider = new PerWebRequestContainerProvider(accessor.Object);
        //    var resolver = new DefaultCommandResolver(provider);

        //    var commandContext = new CommandContextTest();
        //    var command = resolver.ResolveCommand<CommandTest>(commandContext);
        //    Assert.NotNull(command);
        //    Assert.Equal(command.Context, commandContext);
        //}

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
