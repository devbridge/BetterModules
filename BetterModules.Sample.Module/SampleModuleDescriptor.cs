using BetterModules.Core.Modules;

namespace BetterModules.Sample.Module
{
    public class SampleModuleDescriptor : ModuleDescriptor
    {
        public const string ModuleName = "BetterModulesSample";

        public override string Description
        {
            get
            {
                return "Sample BetterModules Module";
            }
        }

        public override string Name
        {
            get
            {
                return ModuleName;
            }
        }
    }
}
