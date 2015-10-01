using System.Security.Principal;
using System.Threading;
using BetterModules.Core.Security;
using Xunit;

namespace BetterModules.Core.Tests.Security
{
    public class DefaultPrincipalProviderTests
    {
        [Fact]
        public void Should_Return_CurrectPrincipal()
        {
            var origPrincipal = Thread.CurrentPrincipal;
            var principal = new GenericPrincipal(new GenericIdentity("TestPrincipal1"), null);
            Thread.CurrentPrincipal = principal;

            var principalProvider = new DefaultPrincipalProvider();
            var retrievedPrincipal = principalProvider.GetCurrentPrincipal();

            Assert.Equal(principal, retrievedPrincipal);

            Thread.CurrentPrincipal = origPrincipal;
        }
        
        [Fact]
        public void Should_Return_CurrectPrincipal_Name()
        {
            var origPrincipal = Thread.CurrentPrincipal;
            var principal = new GenericPrincipal(new GenericIdentity("TestPrincipal2"), null);
            Thread.CurrentPrincipal = principal;

            var principalProvider = new DefaultPrincipalProvider();
            var name = principalProvider.CurrentPrincipalName;

            Assert.Equal(name, "TestPrincipal2");

            Thread.CurrentPrincipal = origPrincipal;
        }
        
        [Fact]
        public void Should_Return_Anonymous_Principal_Name()
        {
            var origPrincipal = Thread.CurrentPrincipal;
            Thread.CurrentPrincipal = null;

            var principalProvider = new DefaultPrincipalProvider();
            var name = principalProvider.CurrentPrincipalName;

            Assert.Equal(name, DefaultPrincipalProvider.AnonymousPrincipalName);

            Thread.CurrentPrincipal = origPrincipal;
        }
    }
}
