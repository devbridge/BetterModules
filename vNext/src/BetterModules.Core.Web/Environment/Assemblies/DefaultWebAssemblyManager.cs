using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;
using IAssemblyLoader = BetterModules.Core.Environment.Assemblies.IAssemblyLoader;

namespace BetterModules.Core.Web.Environment.Assemblies
{
    public class DefaultWebAssemblyManager : DefaultAssemblyManager
    {
        /// <summary>
        /// The embedded resources provider
        /// </summary>
        private readonly IEmbeddedResourcesProvider embeddedResourcesProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebAssemblyManager"/> class.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="modulesRegistration">The modules registration.</param>
        /// <param name="embeddedResourcesProvider">The embedded resources provider.</param>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultWebAssemblyManager(IWorkingDirectory workingDirectory, IModulesRegistration modulesRegistration, 
            IEmbeddedResourcesProvider embeddedResourcesProvider, IAssemblyLoader assemblyLoader, 
            ILibraryManager libraryManager, ILoggerFactory loggerFactory)
            : base(workingDirectory, modulesRegistration, assemblyLoader, libraryManager, loggerFactory)
        {
            this.embeddedResourcesProvider = embeddedResourcesProvider;
        }

        /// <summary>
        /// Adds the uploaded module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public override void AddUploadedModule(Assembly assembly)
        {
            base.AddUploadedModule(assembly);

            //TODO: Check how to add referenced assembly (if we need to do it at all)
            //BuildManager.AddReferencedAssembly(assembly);
            embeddedResourcesProvider.AddEmbeddedResourcesFrom(assembly);
        }

        /// <summary>
        /// Adds the referenced module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public override void AddReferencedModule(Assembly assembly)
        {
            base.AddReferencedModule(assembly);

            embeddedResourcesProvider.AddEmbeddedResourcesFrom(assembly);
        }
    }
}
