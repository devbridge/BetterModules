﻿using System.Reflection;
using System.Web.Compilation;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Web.Web.EmbeddedResources;

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
            IEmbeddedResourcesProvider embeddedResourcesProvider, IAssemblyLoader assemblyLoader)
            : base(workingDirectory, modulesRegistration, assemblyLoader)
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

            BuildManager.AddReferencedAssembly(assembly);
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
