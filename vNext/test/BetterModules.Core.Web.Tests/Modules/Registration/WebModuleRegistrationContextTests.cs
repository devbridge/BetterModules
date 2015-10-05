using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Sample.Web.Module;
using Xunit;

namespace BetterModules.Core.Web.Tests.Modules.Registration
{
    public class WebModuleRegistrationContextTests
    {
        [Fact]
        public void Should_Initialize_Context_Correctly()
        {
            var descriptor = new SampleWebModuleDescriptor();
            var context = new WebModuleRegistrationContext(descriptor);

            Assert.Equal(context.ModuleDescriptor, descriptor);
            Assert.NotNull(context.GetRegistrationName());
        }
    }
}
