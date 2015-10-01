using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Extensions;
using BetterModules.Core.Infrastructure.Commands;
using BetterModules.Core.Modules;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Core.Web.Mvc.Extensions;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Web.Modules
{
    /// <summary>
    /// Abstract web module descriptor. 
    /// </summary>
    public abstract class WebModuleDescriptor : ModuleDescriptor
    {
        private string areaName;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract override string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public abstract override string Description { get; }

        /// <summary>
        /// Gets the name of the module area.
        /// </summary>
        /// <value>
        /// The name of the module area.
        /// </value>
        public virtual string AreaName => areaName ?? (areaName = ("module-" + Name).ToLowerInvariant());

        // From now on custom routes will be registered using attributes
        ///// <summary>
        ///// Registers a routes.
        ///// </summary>
        ///// <param name="context">The area registration context.</param>
        ///// <param name="services"></param>
        //public virtual void RegisterCustomRoutes(WebModuleRegistrationContext context, IServiceCollection services)
        //{
        //}

        // Controller registration to DI container ir no longer needed
        ///// <summary>
        ///// Registers module controller types.
        ///// </summary>
        ///// <param name="registrationContext">The area registration context.</param>
        ///// <param name="services"></param>
        ///// <param name="controllerExtensions">The controller extensions.</param>
        //public virtual void RegisterModuleControllers(WebModuleRegistrationContext registrationContext, IServiceCollection services, IControllerExtensions controllerExtensions)
        //{
        //    var controllerTypes = controllerExtensions.GetControllerTypes(GetType().Assembly);

        //    if (controllerTypes != null)
        //    {
        //        var namespaces = new List<string>();

        //        foreach (Type controllerType in controllerTypes)
        //        {
        //            string key = (AreaName + "-" + controllerType.Name).ToUpperInvariant();
        //            if (!namespaces.Contains(controllerType.Namespace))
        //            {
        //                namespaces.Add(controllerType.Namespace);
        //            }
        //            containerBuilder
        //                .RegisterType(controllerType)
        //                .AsSelf()
        //                .Keyed<IController>(key)
        //                .WithMetadata("ControllerType", controllerType)
        //                .InstancePerDependency()
        //                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
        //        }
        //    }
        //}

        /// <summary>
        /// Registers the module command types.
        /// </summary>
        /// <param name="registrationContext">The area registration context.</param>
        /// <param name="services"></param>
        public virtual void RegisterModuleCommands(WebModuleRegistrationContext registrationContext, IServiceCollection services)
        {
            Assembly assembly = GetType().Assembly;

            Type[] commandTypes = {
                    typeof(ICommand),
                    typeof(ICommandIn<>),
                    typeof(ICommandOut<>),
                    typeof(ICommand<,>)
                };
            var types = assembly
                .GetExportedTypes()
                .Where(type => commandTypes.Any(type.IsAssignableToGenericType))
                .ToList();
            foreach (var type in types)
            {
                services.AddScoped(type);
                var interfaces = type
                    .GetInterfaces()
                    .Where(x => x.IsPublic && x != typeof (IDisposable))
                    .ToList();
                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, type);
                }
            }
        }

        /// <summary>
        /// Creates the registration context.
        /// </summary>
        /// <returns>Module registration context</returns>
        public override ModuleRegistrationContext CreateRegistrationContext()
        {
            return new WebModuleRegistrationContext(this);
        }
    }
}
