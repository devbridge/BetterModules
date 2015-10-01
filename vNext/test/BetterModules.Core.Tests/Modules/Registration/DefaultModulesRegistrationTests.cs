using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules.Registration;
using BetterModules.Sample.Module;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Moq;
using Xunit;

namespace BetterModules.Core.Tests.Modules.Registration
{
    public class DefaultModulesRegistrationTests
    {
        [Fact]
        public void ShouldAddDescriptor_FromAssembly_Correctly()
        {
            AddAssembly<SampleModuleDescriptor>(1);
        }

        [Fact]
        public void ShouldNotAddDescriptor_FromAssembly_Correctly()
        {
            AddAssembly<DefaultModulesRegistration>(0);
        }

        private void AddAssembly<TType>(int expectedResult)
        {
            var assemblyLoaderMock = new Mock<IAssemblyLoader>();
            assemblyLoaderMock
                .Setup(a => a.GetLoadableTypes(It.IsAny<Assembly>()))
                .Returns<Assembly>(a => new List<Type> { typeof(TType) });

            var registration = new DefaultModulesRegistration(assemblyLoaderMock.Object, new LoggerFactory());
            registration.AddModuleDescriptorTypeFromAssembly(typeof(TType).Assembly);

            var services = new ServiceCollection();

            registration.InitializeModules(services);
            var modules = registration.GetModules();

            Assert.NotNull(modules);
            Assert.Equal(modules.Count(), expectedResult);
        }
    }
}
