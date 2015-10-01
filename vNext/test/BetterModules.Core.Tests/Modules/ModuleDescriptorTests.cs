using BetterModules.Core.Modules;
using BetterModules.Core.Modules.Registration;
using Xunit;

namespace BetterModules.Core.Tests.Modules
{
    public class ModuleDescriptorTests
    {
        [Fact]
        public void Should_Return_Correct_Assembly_Name()
        {
            var descriptor = new TestModuleDescriptor();

            Assert.Equal(descriptor.AssemblyName.Name, GetType().Assembly.GetName().Name);
        }
        
        [Fact]
        public void Should_Return_Correct_RegistrationContext()
        {
            var descriptor = new TestModuleDescriptor();
            var context = descriptor.CreateRegistrationContext();

            Assert.NotNull(context);
            Assert.Equal(context.GetType(), typeof(ModuleRegistrationContext));
        }

        private class TestModuleDescriptor : ModuleDescriptor
        {
            public override string Description
            {
                get
                {
                    return "Test";
                }
            }

            public override string Name
            {
                get
                {
                    return "Test";
                }
            }
        }
    }
}
