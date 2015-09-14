using System;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules.Registration;
using FluentNHibernate.Cfg;
using Microsoft.Framework.Logging;

namespace BetterModules.Core.DataAccess.DataContext
{
    public class DefaultMappingResolver : IMappingResolver
    {
        private readonly ILogger logger;
        private readonly IModulesRegistration modulesRegistry;
        private readonly IAssemblyLoader assemblyLoader;

        public DefaultMappingResolver(IModulesRegistration modulesRegistry, IAssemblyLoader assemblyLoader, ILoggerFactory loggerFactory)
        {        
            this.modulesRegistry = modulesRegistry;
            this.assemblyLoader = assemblyLoader;
            logger = loggerFactory.CreateLogger(typeof(DefaultMappingResolver).FullName);
        }

        public void AddAvailableMappings(FluentConfiguration fluentConfiguration)
        {
            foreach (var module in modulesRegistry.GetModules())
            {
                try
                {
                    var assembly = assemblyLoader.Load(module.ModuleDescriptor.AssemblyName);
                    if (assembly != null)
                    {
                        fluentConfiguration = fluentConfiguration.Mappings(m => m.FluentMappings.AddFromAssembly(assembly));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to load mappings from module {0} (assembly {1}).", ex, module.ModuleDescriptor.Name, module.ModuleDescriptor.AssemblyName);
                }
            }
        }
    }
}
