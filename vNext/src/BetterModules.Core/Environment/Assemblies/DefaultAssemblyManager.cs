using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Modules.Registration;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.Environment.Assemblies
{
    /// <summary>
    /// Default assembly manager implementation.
    /// </summary>
    public class DefaultAssemblyManager : IAssemblyManager
    {               
        /// <summary>
        /// Logging contract.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Working directory managing.
        /// </summary>
        private readonly IWorkingDirectory workingDirectory;

        /// <summary>
        /// Modules registry.
        /// </summary>
        private readonly IModulesRegistration modulesRegistration;

        /// <summary>
        /// Assemblies loader.
        /// </summary>
        private readonly IAssemblyLoader assemblyLoader;

        private readonly ILibraryManager libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyManager" /> class.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="modulesRegistration">The module loader.</param>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultAssemblyManager(
            IWorkingDirectory workingDirectory,
            IModulesRegistration modulesRegistration,
            IAssemblyLoader assemblyLoader,
            ILibraryManager libraryManager,
            ILoggerFactory loggerFactory)
        {
            this.workingDirectory = workingDirectory;
            this.modulesRegistration = modulesRegistration;
            this.assemblyLoader = assemblyLoader;
            this.libraryManager = libraryManager;
            logger = loggerFactory.CreateLogger(typeof (DefaultAssemblyManager).FullName);
        }

        /// <summary>
        /// Tries to attach available module assemblies from working modules directory.
        /// </summary>
        public void AddUploadedModules()
        {
            if (logger.IsEnabled(LogLevel.Verbose))
            {
                logger.LogVerbose("Add uploaded modules.");
            }

            var availableModuleFiles = workingDirectory.GetAvailableModules();
            var runtimeModuleFiles = new List<FileInfo>();

            foreach (var moduleInfo in availableModuleFiles)
            {
                try
                {
                    var runtimeModuleInfo = workingDirectory.RecopyModulesToRuntimeFolder(moduleInfo);
                    runtimeModuleFiles.Add(runtimeModuleInfo);
                }
                catch (Exception ex)
                {
                    throw new CoreException("Failed to recopy module " + moduleInfo.FullName + ".", ex);
                }
            }

            foreach (var runtimeModuleFile in runtimeModuleFiles)
            {
                try
                {                    
                    if (runtimeModuleFile.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        var runtimeModuleAssembly = assemblyLoader.Load(AssemblyName.GetAssemblyName(runtimeModuleFile.FullName));

                        AddUploadedModule(runtimeModuleAssembly);
                    }
                }
                catch (Exception ex)
                {
                    throw new CoreException("Failed to add reference to runtime module " + runtimeModuleFile.FullName + ".", ex);
                }
            }
        }

        /// <summary>
        /// Adds the uploaded module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public virtual void AddUploadedModule(Assembly assembly)
        {
            modulesRegistration.AddModuleDescriptorTypeFromAssembly(assembly);
        }

        /// <summary>
        /// Adds referenced modules.
        /// </summary>
        public void AddReferencedModules()
        {
            if (logger.IsEnabled(LogLevel.Verbose))
            {
                logger.LogVerbose("Add referenced modules.");
            }

            var libraries = libraryManager.GetReferencingLibraries("BetterModules.Core");
            foreach (var library in libraries)
            {
                try
                {
                    var assemblies = library.Assemblies;
                    foreach (var assembly in assemblies)
                    {
                        var module = assemblyLoader.Load(assembly);
                        AddReferencedModule(module);
                    }                
                }
                catch (BadImageFormatException ex)
                {
                    logger.LogError("Bad assembly format", ex);
                }
            }

        }

        /// <summary>
        /// Adds the referenced module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public virtual void AddReferencedModule(Assembly assembly)
        {
            modulesRegistration.AddModuleDescriptorTypeFromAssembly(assembly);
        }
    }
}
