using BetterModules.Core.Web.Mvc.Extensions;
using Xunit;

namespace BetterModules.Core.Web.Tests.Mvc.Extensions
{
    public class VirtualPathTests
    {
        [Fact]
        public void Should_Combine_Path_Correctly()
        {
            var path = VirtualPath.Combine("c:\\a", "b", "f");

            Assert.Equal(path, "c:/a/b/f");
        }
        
        [Fact]
        public void Should_Resolve_Local_Path_Correctly()
        {
            Assert.True(VirtualPath.IsLocalPath("local"));
            Assert.True(VirtualPath.IsLocalPath("(local)"));
            Assert.False(VirtualPath.IsLocalPath("http://www.google.com"));
            Assert.False(VirtualPath.IsLocalPath("other"));
        }
    }
}
