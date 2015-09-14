using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using Microsoft.Framework.Logging;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Modules.Registration
{
    /// <summary>
    /// Default modules registration implementation.
    /// </summary>
    public class DefaultModulesRegistration : IModulesRegistration
    {
        /// <summary>
        /// Current class logger.
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Assembly loader instance.
        /// </summary>
        protected readonly IAssemblyLoader assemblyLoader;

        /// <summary>
        /// Thread safe known module types.
        /// </summary>
        protected readonly Dictionary<string, Type> knownModuleDescriptorTypes;

        /// <summary>
        /// Thread safe modules dictionary.
        /// </summary>
        protected readonly Dictionary<string, ModuleRegistrationContext> knownModules;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModulesRegistration" /> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultModulesRegistration(IAssemblyLoader assemblyLoader, ILoggerFactory loggerFactory)
        {
            this.assemblyLoader = assemblyLoader;
            logger = loggerFactory.CreateLogger(typeof(DefaultModulesRegistration).FullName);

            knownModuleDescriptorTypes = new Dictionary<string, Type>();
            knownModules = new Dictionary<string, ModuleRegistrationContext>();
        }

        /// <summary>
        /// Tries to scan and adds module descriptor type from assembly.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        public void AddModuleDescriptorTypeFromAssembly(Assembly assembly)
        {
            if (logger.IsEnabled(LogLevel.Verbose))
            {
                logger.LogVerbose("Searching for module descriptor type in the assembly {0}.", assembly.FullName);
            }

            var moduleRegistrationType = assemblyLoader.GetLoadableTypes(assembly).Where(IsModuleDescriptorType).FirstOrDefault();
            if (moduleRegistrationType != null)
            {
                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    logger.LogVerbose("Adds module descriptor {0} from the assembly {1}.", moduleRegistrationType.Name, assembly.FullName);
                }

                if (!knownModuleDescriptorTypes.ContainsKey(moduleRegistrationType.Name))
                {
                    knownModuleDescriptorTypes.Add(moduleRegistrationType.Name, moduleRegistrationType);
                }
                else
                {
                    logger.LogInformation("Module descriptor {0} from the assembly {1} already included.", moduleRegistrationType.Name, assembly.FullName);
                }
            }
        }

        /// <summary>
        /// Gets registered modules.
        /// </summary>
        /// <returns>List of registered modules.</returns>
        public IEnumerable<ModuleRegistrationContext> GetModules()
        {
            return knownModules.Values;
        }

        /// <summary>
        /// Registers the module.
        /// </summary>
        /// <param name="moduleDescriptor">Module information.</param>
        /// <param name="services">The collection of services</param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private void RegisterModule(ModuleDescriptor moduleDescriptor, IServiceCollection services)
        {
            //ContainerBuilder containerBuilder = new ContainerBuilder();

            var registrationContext = moduleDescriptor.CreateRegistrationContext();

            RegisterModuleDescriptor(registrationContext, services);
            //ContextScopeProvider.RegisterTypes(containerBuilder);

            knownModules.Add(registrationContext.GetRegistrationName(), registrationContext);
        }

        /// <summary>
        /// Registers the types.
        /// </summary>
        /// <param name="registrationContext">The registration context.</param>
        /// <param name="services">The collection of services.</param>
        protected virtual void RegisterModuleDescriptor(ModuleRegistrationContext registrationContext, IServiceCollection services)
        {
            registrationContext.ModuleDescriptor.RegisterModuleTypes(registrationContext, services);
        }

        /// <summary>
        /// Initializes all known modules.
        /// </summary>        
        public virtual void InitializeModules(IServiceCollection services)
        {
            if (knownModuleDescriptorTypes != null && knownModuleDescriptorTypes.Count > 0)
            {
                //ContainerBuilder containerBuilder = new ContainerBuilder();
                foreach (var moduleDescriptorType in knownModuleDescriptorTypes.Values)
                {
                    //containerBuilder.RegisterType(moduleDescriptorType).AsSelf().SingleInstance();
                    services.AddSingleton(moduleDescriptorType);
                }

                //ContextScopeProvider.RegisterTypes(containerBuilder);

                var provider = services.BuildServiceProvider();

                var moduleDescriptors = new List<ModuleDescriptor>();
                foreach (var moduleDescriptorType in knownModuleDescriptorTypes.Values)
                {
                    if (services.Any(x => x.Lifetime == ServiceLifetime.Singleton
                                          && x.ServiceType == moduleDescriptorType
                                          && x.ImplementationType == moduleDescriptorType))
                    {
                        var moduleDescriptor = provider.GetService(moduleDescriptorType) as ModuleDescriptor;
                        moduleDescriptors.Add(moduleDescriptor);
                    }
                    else
                    {
                        logger.LogWarning("Failed to resolve module instance from type {0}.", moduleDescriptorType.FullName);
                    }
                }

                moduleDescriptors = moduleDescriptors.OrderBy(f => f.Order).ToList();
                foreach (var moduleDescriptor in moduleDescriptors)
                {
                    try
                    {
                        RegisterModule(moduleDescriptor, services);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("Failed to register module of type {0}.", ex, moduleDescriptor.GetType().FullName);
                    }
                }

            }
            else
            {
                logger.LogInformation("No registered module descriptors found.");
            }
        }

        /// <summary>
        /// Determines whether type is module descriptor type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if type is module descriptor type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsModuleDescriptorType(Type type)
        {
            return typeof(ModuleDescriptor).IsAssignableFrom(type) && type.IsPublic && !type.IsAbstract;
        }
    }
}
