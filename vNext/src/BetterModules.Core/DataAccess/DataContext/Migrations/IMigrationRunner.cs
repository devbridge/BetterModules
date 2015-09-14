using System.Collections.Generic;
using BetterModules.Core.Modules;

namespace BetterModules.Core.DataAccess.DataContext.Migrations
{
    /// <summary>
    /// Defines contract to run database migrations.
    /// </summary>
    public interface IMigrationRunner
    {
        /// <summary>
        /// Runs migrations from the specified modules.
        /// </summary>
        void MigrateStructure(IList<ModuleDescriptor> moduleDescriptors);
    }
}
