//using System.Linq;
//using System.Reflection;
//using BetterModules.Core.Environment.Assemblies;
//using Microsoft.Framework.Logging;
//using Xunit;

//namespace BetterModules.Core.Tests.Environment.Assemblies
//{
//    public class DefaultAssemblyLoaderTests
//    {
//        [Fact]
//        public void ShouldLoad_Assembly_FileByStringName()
//        {
//            var loader = new DefaultAssemblyLoader(new LoggerFactory());
//            var assembly = loader.LoadFromFile("BetterModules.Sample.Module.dll");

//            Assert.NotNull(assembly);
//            Assert.True(assembly.FullName.StartsWith("BetterModules.Sample.Module"));
//        }

//        [Fact]
//        public void ShouldLoad_Assembly_FileByAssemblyName()
//        {
//            var loader = new DefaultAssemblyLoader(new LoggerFactory());
//            var assemblyName = AssemblyName.GetAssemblyName("BetterModules.Sample.Module.dll");
//            var assembly = loader.Load(assemblyName);

//            Assert.NotNull(assembly);
//            Assert.True(assembly.FullName.StartsWith("BetterModules.Sample.Module"));
//        }

//        [Fact]
//        public void ShouldLoad_Assembly_GetLoadableTypes()
//        {
//            var type = GetType();
//            var assembly = type.Assembly;
//            var loader = new DefaultAssemblyLoader(new LoggerFactory());
//            var types = loader.GetLoadableTypes(assembly);

//            Assert.NotNull(types);
//            Assert.True(types.Count() > 1);
//            Assert.True(types.Contains(GetType()));
//        }

//        //[Fact]
//        //public void ShouldLoad_Assembly_GetLoadableTypes_Generic()
//        //{
//        //    var type = GetType();
//        //    var assembly = type.Assembly;
//        //    var loader = new DefaultAssemblyLoader(new LoggerFactory());
//        //    var testBaseType = typeof(TestBase);
//        //    var types = loader.GetLoadableTypes(assembly, testBaseType);

//        //    Assert.NotNull(types);
//        //    Assert.Greater(types.Count(), 1);
//        //    Assert.True(types.Contains(GetType()));
//        //    Assert.True(types.All(testBaseType.IsAssignableFrom));
//        //}
//    }
//}
