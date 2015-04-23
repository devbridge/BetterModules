using System;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Modules.Registration;
using FluentNHibernate.Mapping;

namespace BetterModules.Core.Models
{
    /// <summary>
    /// Fluent nHibernate sub-entity base map class.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to map.</typeparam>
    public abstract class EntitySubClassMapBase<TEntity> : SubclassMap<TEntity>
    {
        /// <summary>
        /// The module name.
        /// </summary>
        private readonly string moduleName;

        /// <summary>
        /// The schema name
        /// </summary>
        private string schemaName;

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>
        /// The name of the schema.
        /// </value>
        protected string SchemaName
        {
            get
            {
                return schemaName ?? (schemaName = SchemaNameProvider.GetSchemaName(moduleName));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySubClassMapBase{TEntity}" /> class.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        protected EntitySubClassMapBase(string moduleName)
        {
            this.moduleName = moduleName;
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules().First(module => module.ModuleDescriptor.Name == moduleName);
            schemaName = currentModule.ModuleDescriptor.SchemaName;
            Init();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySubClassMapBase{TEntity}"/> class.
        /// </summary>
        /// <param name="moduleDescriptorType">Type of the module descriptor.</param>
        protected EntitySubClassMapBase(Type moduleDescriptorType)
        {
            var currentModule =
                ModulesRegistrationSingleton.Instance.GetModules()
                    .First(module => module.ModuleDescriptor.GetType() == moduleDescriptorType);
            schemaName = currentModule.ModuleDescriptor.SchemaName;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySubClassMapBase{TEntity}"/> class.
        /// </summary>
        protected EntitySubClassMapBase()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var currentModule =
                ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(module => module.ModuleDescriptor.AssemblyName == assembly.GetName());
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
            Init();
        }

        private void Init()
        {
            if (SchemaName != null)
            {
                Schema(SchemaName);
            }

            KeyColumn("Id");
        }
    }
}
