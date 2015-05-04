using System;
using System.Linq;
using BetterModules.Core.DataContracts;
using BetterModules.Core.Modules.Registration;
using FluentNHibernate.Mapping;

namespace BetterModules.Core.Models
{
    /// <summary>
    /// Fluent nHibernate entity map base class.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to map.</typeparam>
    public abstract class EntityMapBase<TEntity> : ClassMap<TEntity>
        where TEntity : IEntity
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
        /// Initializes a new instance of the <see cref="EntityMapBase{TEntity}" /> class.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        protected EntityMapBase(string moduleName)
        {
            this.moduleName = moduleName;
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                .FirstOrDefault(
                    module => module.ModuleDescriptor != null && module.ModuleDescriptor.Name == moduleName);
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapBase{TEntity}"/> class.
        /// </summary>
        /// <param name="moduleDescriptorType">Type of the module descriptor.</param>
        protected EntityMapBase(Type moduleDescriptorType)
        {
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(
                        module => module.ModuleDescriptor != null && module.ModuleDescriptor.GetType() == moduleDescriptorType);
            if (currentModule != null)
            {
                schemaName = currentModule.ModuleDescriptor.SchemaName;
            }
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapBase{TEntity}"/> class.
        /// </summary>
        protected EntityMapBase()
        {
            var assembly = this.GetType().Assembly;
            var currentModule = ModulesRegistrationSingleton.Instance.GetModules()
                    .FirstOrDefault(
                        module => module.ModuleDescriptor != null && module.ModuleDescriptor.AssemblyName == assembly.GetName());
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

            Id(x => x.Id).GeneratedBy.Custom<EntityIdGenerator>();

            Map(x => x.IsDeleted).Not.Nullable();

            Map(x => x.CreatedOn).Not.Nullable();
            Map(x => x.ModifiedOn).Not.Nullable();
            Map(x => x.DeletedOn).Nullable();

            Map(x => x.CreatedByUser).Not.Nullable().Length(MaxLength.Name);
            Map(x => x.ModifiedByUser).Not.Nullable().Length(MaxLength.Name);
            Map(x => x.DeletedByUser).Nullable().Length(MaxLength.Name);

            Version(x => x.Version);

            OptimisticLock.Version();
        }
    }
}
