using System;
using System.IO;
using System.Linq;
using BetterModules.Core.Environment.FileSystem;
using Microsoft.Framework.Logging;
using Xunit;

namespace BetterModules.Core.Tests.Environment.FileSystem
{
    public class DefaultWorkingDirectoryTests
    {
        private string OriginalFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "TestModules", "test.dll");

        private string RuntimeFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.dll");

        private string ModuleFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Modules", "test.dll");

        [Fact]
        public void ShouldReturn_Correct_WorkingDirectoryPath()
        {
            var service = new DefaultWorkingDirectory(new LoggerFactory());
            var path = service.GetWorkingDirectoryPath();

            Assert.NotNull(path);
            Assert.True(path.Contains(AppDomain.CurrentDomain.BaseDirectory));
        }
        
        [Fact]
        public void ShouldReturn_AvailableModules()
        {
            PrepareTestDll();

            var service = new DefaultWorkingDirectory(new LoggerFactory());
            var modules = service.GetAvailableModules();

            Assert.NotNull(modules);
            Assert.Equal(modules.Count(), 1);
            Assert.Equal(modules.First().Name, "test.dll");

            RemoveTestDll();
        }
        
        [Fact]
        public void ShouldCopy_ModulesToRuntimeDirectory_Successfully()
        {
            PrepareTestDll();

            var service = new DefaultWorkingDirectory(new LoggerFactory());
            service.RecopyModulesToRuntimeFolder(new FileInfo(ModuleFileName));
            Assert.True(File.Exists(RuntimeFileName));
            
            RemoveTestDll();
        }

        private void PrepareTestDll()
        {
            RemoveTestDll();

            var directory = Path.GetDirectoryName(ModuleFileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(ModuleFileName))
            {
                File.Copy(OriginalFileName, ModuleFileName);
            }
        }

        private void RemoveTestDll()
        {
            if (File.Exists(RuntimeFileName))
            {
                File.Delete(RuntimeFileName);
            }
            if (File.Exists(ModuleFileName))
            {
                File.Delete(ModuleFileName);
            }
        }
    }
}
