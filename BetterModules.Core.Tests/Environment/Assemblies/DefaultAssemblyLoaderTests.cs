using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Environment.Assemblies
{
    [TestFixture]
    public class DefaultAssemblyLoaderTests : TestBase
    {
        [Test]
        public void ShouldLoad_Assembly_FileByStringName()
        {
            var loader = new DefaultAssemblyLoader();
            var assembly = loader.LoadFromFile("BetterModules.Sample.Module.dll");

            Assert.IsNotNull(assembly);
            Assert.IsTrue(assembly.FullName.StartsWith("BetterModules.Sample.Module"));
        }

        [Test]
        public void ShouldLoad_Assembly_FileByAssemblyName()
        {
            var loader = new DefaultAssemblyLoader();
            var assemblyName = AssemblyName.GetAssemblyName("BetterModules.Sample.Module.dll");
            var assembly = loader.Load(assemblyName);

            Assert.IsNotNull(assembly);
            Assert.IsTrue(assembly.FullName.StartsWith("BetterModules.Sample.Module"));
        }

        [Test]
        public void ShouldLoad_Assembly_GetLoadableTypes()
        {
            var type = GetType();
            var assembly = type.Assembly;
            var loader = new DefaultAssemblyLoader();
            var types = loader.GetLoadableTypes(assembly);

            Assert.IsNotNull(types);
            Assert.Greater(types.Count(), 1);
            Assert.IsTrue(types.Contains(GetType()));
        }

        [Test]
        public void ShouldLoad_Assembly_GetLoadableTypes_Generic()
        {
            var type = GetType();
            var assembly = type.Assembly;
            var loader = new DefaultAssemblyLoader();
            var testBaseType = typeof(TestBase);
            var types = loader.GetLoadableTypes(assembly, testBaseType);

            Assert.IsNotNull(types);
            Assert.Greater(types.Count(), 1);
            Assert.IsTrue(types.Contains(GetType()));
            Assert.IsTrue(types.All(testBaseType.IsAssignableFrom));
        }
    }
}
