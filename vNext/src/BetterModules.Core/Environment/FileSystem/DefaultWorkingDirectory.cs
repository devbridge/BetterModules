using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.Environment.FileSystem
{
    /// <summary>
    /// Default working directory managing implementation.
    /// </summary>
    public class DefaultWorkingDirectory : IWorkingDirectory
    {        
        /// <summary>
        /// Modules folder name.
        /// </summary>
        private const string ModulesFolderName = "Modules";

        /// <summary>
        /// Current class logger.
        /// </summary>
        private readonly ILogger logger;
        
        /// <summary>
        /// Physical root folder.
        /// </summary>
        private readonly DirectoryInfo rootFolder;

        /// <summary>
        /// Physical modules folder.
        /// </summary>
        private readonly DirectoryInfo modulesFolder;

        /// <summary>
        /// Physical modules shadow copy folder.
        /// </summary>
        private readonly DirectoryInfo modulesRuntimeFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWorkingDirectory" /> class.
        /// </summary>
        public DefaultWorkingDirectory(ILoggerFactory loggerFactory)
        {
            rootFolder = new DirectoryInfo(GetWorkingDirectoryPath());

            modulesFolder = new DirectoryInfo(Path.Combine(rootFolder.FullName, ModulesFolderName));
            modulesRuntimeFolder = new DirectoryInfo(AppDomain.CurrentDomain.DynamicDirectory ?? AppDomain.CurrentDomain.BaseDirectory);

            logger = loggerFactory.CreateLogger(typeof (DefaultWorkingDirectory).FullName);
        }

        /// <summary>
        /// Gets the working directory path.
        /// </summary>
        /// <returns>The working directory path</returns>
        public virtual string GetWorkingDirectoryPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        }

        /// <summary>
        /// Gets module files from working folder.
        /// </summary>        
        /// <returns>Assembly file names.</returns>
        public virtual IEnumerable<FileInfo> GetAvailableModules()
        {                        
            if (!modulesFolder.Exists)
            {
                return Enumerable.Empty<FileInfo>();
            }

            return modulesFolder.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Tries to recopy a module assembly to runtime folder.
        /// </summary>
        /// <param name="module">The module file information.</param>
        /// <returns>Runtime module file information.</returns>
        public virtual FileInfo RecopyModulesToRuntimeFolder(FileInfo module)
        {             
            if (!modulesRuntimeFolder.Exists)
            {
                modulesRuntimeFolder.Create();
            }

            var runtimeModule = new FileInfo(Path.Combine(modulesRuntimeFolder.FullName, module.Name));
            
            try
            {
                File.Copy(module.FullName, runtimeModule.FullName, true);
            }
            catch (IOException)
            {
                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    logger.LogVerbose("Warning! Can't recopy the {0} assembly is locked. Try to recopy it after rename hack.", runtimeModule.FullName);
                }

                try
                {
                    var temporaryFile = runtimeModule.FullName + Guid.NewGuid().ToString("N") + ".tmp";
                    File.Move(runtimeModule.FullName, temporaryFile);                    
                }
                catch (IOException inner)
                {
                    if (logger.IsEnabled(LogLevel.Verbose))
                    {
                        logger.LogVerbose("Failed to rename the assembly {0} file.", inner, runtimeModule.FullName);
                    }

                    throw;
                }

                File.Copy(module.FullName, runtimeModule.FullName, true);

                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    logger.LogVerbose("The {0} assembly successfully renamed and copied.", runtimeModule.FullName);
                }
            }

            return runtimeModule;
        }
    }
}
