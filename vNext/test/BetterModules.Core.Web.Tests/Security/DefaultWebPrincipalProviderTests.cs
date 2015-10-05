using System.Security.Principal;
using System.Threading;
using BetterModules.Core.Web.Security;
using BetterModules.Core.Web.Tests.TestHelpers;
using BetterModules.Core.Web.Web;
using Moq;
using Xunit;

namespace BetterModules.Core.Web.Tests.Security
{
    public class DefaultWebPrincipalProviderTests
    {
        [Fact]
        public void Should_Return_Current_Principal()
        {
            var accessor = new Mock<IHttpContextAccessor>();
            var contextMock = new HttpContextMoq();
            var fakePrincipal = new GenericPrincipal(new GenericIdentity("TEST"), null);
            contextMock.MockContext.Setup(r => r.User).Returns(() => fakePrincipal);
            accessor.Setup(r => r.HttpContext).Returns(() => contextMock.MockContext.Object);

            var provider = new DefaultWebPrincipalProvider(accessor.Object);
            var principal = provider.GetCurrentPrincipal();

            Assert.Equal(principal, fakePrincipal);
        }

        [Fact]
        public void Should_Return_Base_Principal()
        {
            var currentPrincipal = Thread.CurrentPrincipal;
            var fakePrincipal = new GenericPrincipal(new GenericIdentity("TEST"), null);
            Thread.CurrentPrincipal = fakePrincipal;

            var accessor = new Mock<IHttpContextAccessor>();
            var provider = new DefaultWebPrincipalProvider(accessor.Object);

            var principal = provider.GetCurrentPrincipal();

            Assert.NotNull(principal);
            Assert.Equal(principal, fakePrincipal);

            Thread.CurrentPrincipal = currentPrincipal;
        }
    }
}
