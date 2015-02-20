using BetterModules.Core.Modules.Registration;
using BetterModules.Sample.Module;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Modules.Registration
{
    [TestFixture]
    public class ModuleRegistrationContextTests : TestBase
    {
        [Test]
        public void Should_Initialize_Context_Correctly()
        {
            var descriptor = new SampleModuleDescriptor();
            var context = new ModuleRegistrationContext(descriptor);

            Assert.AreEqual(context.ModuleDescriptor, descriptor);
            Assert.IsNotNull(context.GetRegistrationName());
        }
    }
}
