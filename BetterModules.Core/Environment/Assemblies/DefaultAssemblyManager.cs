using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Modules;
using BetterModules.Core.Modules.Registration;
using Common.Logging;

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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyManager" /> class.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="modulesRegistration">The module loader.</param>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultAssemblyManager(
            IWorkingDirectory workingDirectory,
            IModulesRegistration modulesRegistration,
            IAssemblyLoader assemblyLoader)
        {
            this.workingDirectory = workingDirectory;
            this.modulesRegistration = modulesRegistration;
            this.assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Tries to attach available module assemblies from working modules directory.
        /// </summary>
        public void AddUploadedModules()
        {
            if (Log.IsTraceEnabled)
            {
                Log.Trace("Add uploaded modules.");
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
            if (Log.IsTraceEnabled)
            {
                Log.Trace("Add referenced modules.");
            }

            var modules = new List<Assembly>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic)
                {
                    continue;
                }
                try
                {
                    if (asm.GetTypes().Any(t => typeof (ModuleDescriptor).IsAssignableFrom(t)))
                    {
                        modules.Add(asm);
                    }
                }
                catch
                {
                    // Ignore
                }
            }
            var loadedPaths = modules.Select(f => Path.GetFileName(f.Location)).ToArray();

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            if (!directory.ToLowerInvariant().Contains("\\bin"))
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            }

            var referencedPaths = Directory.GetFiles(directory, "*.dll");
            var notLoadedReferencedPaths = referencedPaths.Where(r => !loadedPaths.Contains(Path.GetFileName(r), StringComparer.OrdinalIgnoreCase)).ToList();

            var domain = AppDomain.CreateDomain("tempAssemblyHolder");

            foreach (var notLoadedReferencedPath in notLoadedReferencedPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(notLoadedReferencedPath);
                if (fileName != null )
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(notLoadedReferencedPath);

                    try
                    {
                        var module = domain.Load(assemblyName);
                        if (module.GetTypes().Any(t => typeof(ModuleDescriptor).IsAssignableFrom(t)))
                        {
                            assemblyLoader.Load(assemblyName);
                            modules.Add(module);
                        }
                    }
                    catch
                    {
                        // Just ignore it
                    }
                }
            }

            AppDomain.Unload(domain);

            foreach (var module in modules)
            {
                AddReferencedModule(module);
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
