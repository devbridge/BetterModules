﻿using System.Reflection;

namespace BetterModules.Core.Environment.Assemblies
{
    /// <summary>
    /// Defines the contract to scan and load assemblies.
    /// </summary>
    public interface IAssemblyManager
    {
        /// <summary>
        /// Loads all available assemblies from working directory.
        /// </summary>
        void AddUploadedModules();

        /// <summary>
        /// Adds referenced modules.
        /// </summary>
        void AddReferencedModules();

        /// <summary>
        /// Adds the uploaded module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void AddUploadedModule(Assembly assembly);

        /// <summary>
        /// Adds the referenced module.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void AddReferencedModule(Assembly assembly);
    }
}
