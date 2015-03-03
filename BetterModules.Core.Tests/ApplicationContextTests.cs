using System.Linq;
using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Security;
using BetterModules.Sample.Module;
using NUnit.Framework;

namespace BetterModules.Core.Tests
{
    [TestFixture]
    public class ApplicationContextTests : TestBase
    {
        [Test]
        public void Dependecies_Should_Be_Initialized_Correctly()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                Assert.IsNotNull(container.Resolve<IConfiguration>());
                Assert.IsNotNull(container.Resolve<IModulesRegistration>());
                Assert.IsNotNull(container.Resolve<ISessionFactoryProvider>());
                Assert.IsNotNull(container.Resolve<IAssemblyLoader>());
                Assert.IsNotNull(container.Resolve<IUnitOfWorkFactory>());
                Assert.IsNotNull(container.Resolve<IMappingResolver>());
                Assert.IsNotNull(container.Resolve<IWorkingDirectory>());
                Assert.IsNotNull(container.Resolve<IFetchingProvider>());
                Assert.IsNotNull(container.Resolve<IUnitOfWork>());
                Assert.IsNotNull(container.Resolve<IRepository>());
                Assert.IsNotNull(container.Resolve<IVersionChecker>());
                Assert.IsNotNull(container.Resolve<IMigrationRunner>());
                Assert.IsNotNull(container.Resolve<IPrincipalProvider>());
                Assert.IsNotNull(container.Resolve<IAssemblyManager>());
                
                Assert.AreEqual(container.Resolve<IConfiguration>().GetType(), typeof(DefaultConfigurationSection));
                Assert.AreEqual(container.Resolve<IModulesRegistration>().GetType(), typeof(DefaultModulesRegistration));
                Assert.AreEqual(container.Resolve<IPrincipalProvider>().GetType(), typeof(DefaultPrincipalProvider));
            }
        }

        [Test]
        public void Correct_Modules_Should_Be_Loaded()
        {
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var modulesRegistration = container.Resolve<IModulesRegistration>();
                var modules = modulesRegistration.GetModules();

                Assert.IsNotNull(modules);
                Assert.AreEqual(modules.Count(), 1);
                Assert.AreEqual(modules.First().ModuleDescriptor.GetType(), typeof(SampleModuleDescriptor));
            }
        }

        [Test]
        public void Should_Initialize_Logger_Successfully()
        {
            var logger = Application.InitializeLogger();

            Assert.IsNotNull(logger);
        }
    }
}
