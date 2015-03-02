using System;
using BetterModules.Core.Modules;

namespace BetterModules.Sample.Module
{
    public class SampleModuleDescriptor : ModuleDescriptor
    {
        public static Guid TestItemModelId = new Guid("1AFC3CAC-0B3D-450E-9BEC-7B29FC4487FB");
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
