using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;
using Moq;
using Xunit;
using IAssemblyLoader = BetterModules.Core.Environment.Assemblies.IAssemblyLoader;

namespace BetterModules.Core.Tests.Environment.Assemblies
{
    public class DefaultAssemblyManagerTests
    {
        [Fact]
        public void ShouldAddReferencedModulesCorrectly()
        {
            var registrationMock = new Mock<IModulesRegistration>();
            var assemblyLoaderMock = new Mock<IAssemblyLoader>();
            var workingDirectoryMock = new Mock<IWorkingDirectory>();
            var libraryManegerMock = new Mock<ILibraryManager>();

            var allAssemblies = new List<string>();

            assemblyLoaderMock
                .Setup(r => r.Load(It.IsAny<AssemblyName>()))
                .Callback<AssemblyName>(a => allAssemblies.Add(a.Name));

            registrationMock
                .Setup(r => r.AddModuleDescriptorTypeFromAssembly(It.IsAny<Assembly>()))
                .Callback<Assembly>(a =>
                {
                    if (a != null)
                    {
                        allAssemblies.Add(a.FullName);
                    }
                });

            libraryManegerMock
                .Setup(r => r.GetReferencingLibraries("BetterModules.Core"))
                .Returns(new List<Library>
                {
                    new Library(
                        "BetterModules.Sample.Module", 
                        "1.0.0", "", "", 
                        new List<string> { "BetterModules.Core" }, 
                        new List<AssemblyName>
                        {
                            new AssemblyName("BetterModules.Sample.Module")
                        })
                });

            var manager = new DefaultAssemblyManager(workingDirectoryMock.Object, registrationMock.Object, assemblyLoaderMock.Object, libraryManegerMock.Object, new LoggerFactory());
            manager.AddReferencedModules();

            Assert.True(allAssemblies.Any(a => a.Contains("BetterModules.Sample.Module")));
        }

        /// <summary>
        /// Should retrieve the lists of modules from working directory, copy them to bin folder
        /// and load them using assembly loader
        /// </summary>
        [Fact]
        public void ShouldAddUploadedModulesCorrectly()
        {
            var registrationMock = new Mock<IModulesRegistration>();
            var assemblyLoaderMock = new Mock<IAssemblyLoader>();
            var workingDirectoryMock = new Mock<IWorkingDirectory>();
            var libraryManegerMock = new Mock<ILibraryManager>();

            var assemblyLoaded = false;
            var assemblyCopied = false;

            var files = new[] { new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "TestModules", "test.dll")) };
            workingDirectoryMock
                .Setup(d => d.GetAvailableModules())
                .Returns(() => files);

            workingDirectoryMock
                .Setup(d => d.RecopyModulesToRuntimeFolder(It.IsAny<FileInfo>()))
                .Returns<FileInfo>(
                    file =>
                    {
                        assemblyCopied = true;
                        Assert.Equal(files[0], file);

                        return file;
                    });

            assemblyLoaderMock
                .Setup(r => r.Load(It.IsAny<AssemblyName>()))
                .Callback<AssemblyName>(
                    file =>
                    {
                        assemblyLoaded = true;
                        Assert.Equal(file.Name, "ClassLibrary1");
                    });

            var manager = new DefaultAssemblyManager(workingDirectoryMock.Object, registrationMock.Object, assemblyLoaderMock.Object, libraryManegerMock.Object, new LoggerFactory());
            manager.AddUploadedModules();

            Assert.True(assemblyLoaded);
            Assert.True(assemblyCopied);
        }
    }
}
