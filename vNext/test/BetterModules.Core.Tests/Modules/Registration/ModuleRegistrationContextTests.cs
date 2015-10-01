using BetterModules.Core.Modules.Registration;
using BetterModules.Sample.Module;
using Xunit;

namespace BetterModules.Core.Tests.Modules.Registration
{
    public class ModuleRegistrationContextTests
    {
        [Fact]
        public void Should_Initialize_Context_Correctly()
        {
            var descriptor = new SampleModuleDescriptor();
            var context = new ModuleRegistrationContext(descriptor);

            Assert.Equal(context.ModuleDescriptor, descriptor);
            Assert.NotNull(context.GetRegistrationName());
        }
    }
}
