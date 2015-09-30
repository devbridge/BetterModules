namespace BetterModules.Core.Modules.Registration
{
    public class ModuleRegistrationContext
    {
        public ModuleRegistrationContext(ModuleDescriptor moduleDescriptor)
        {
            ModuleDescriptor = moduleDescriptor;
        }

        public virtual ModuleDescriptor ModuleDescriptor { get; }

        public virtual string GetRegistrationName()
        {
            return ModuleDescriptor.Name.ToLowerInvariant();
        }
    }
}