using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Web.Modules;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc.Extensions;
using BetterModules.Sample.Web.Module;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Moq;
using Xunit;

namespace BetterModules.Core.Web.Tests.Modules.Registration
{
    public class DefaultWebModulesRegistrationTests
    {
        [Fact]
        public void Should_Find_Module_By_AreaName()
        {
            var loader = new Mock<IAssemblyLoader>();
            loader
                .Setup(l => l.GetLoadableTypes(It.IsAny<Assembly>()))
                .Returns<Assembly>(r => new[] { typeof(SampleWebModuleDescriptor) });

            var service = new DefaultWebModulesRegistration(loader.Object, new LoggerFactory());
            service.AddModuleDescriptorTypeFromAssembly(GetType().Assembly);
            service.InitializeModules(new ServiceCollection());

            var sampleDescriptor = new SampleWebModuleDescriptor();

            WebModuleDescriptor descriptor = service.FindModuleByAreaName(sampleDescriptor.AreaName);
            Assert.NotNull(descriptor);
            Assert.Equal(descriptor.Name, sampleDescriptor.Name);
        }
        
        [Fact]
        public void Should_Not_Find_Module_By_AreaName()
        {
            var service = new DefaultWebModulesRegistration(new Mock<IAssemblyLoader>().Object, new LoggerFactory());
            var descriptor = service.FindModuleByAreaName("Test");

            Assert.Null(descriptor);
        }

        [Fact]
        public void Should_Find_Is_Module_Registered_By_AreaName()
        {
            var loader = new Mock<IAssemblyLoader>();
            loader
                .Setup(l => l.GetLoadableTypes(It.IsAny<Assembly>()))
                .Returns<Assembly>(r => new[] { typeof(SampleWebModuleDescriptor) });

            var service = new DefaultWebModulesRegistration(loader.Object, new LoggerFactory());
            service.AddModuleDescriptorTypeFromAssembly(GetType().Assembly);
            service.InitializeModules(new ServiceCollection());

            var sampleDescriptor = new SampleWebModuleDescriptor();

            var isRegisteted = service.IsModuleRegisteredByAreaName(sampleDescriptor.AreaName);
            Assert.True(isRegisteted);
        }

        [Fact]
        public void Should_Not_Find_Is_Module_Registered_By_AreaName()
        {
            var service = new DefaultWebModulesRegistration(new Mock<IAssemblyLoader>().Object, new LoggerFactory());
            var isRegisteted = service.IsModuleRegisteredByAreaName("Test");

            Assert.False(isRegisteted);
        }
    }
}
