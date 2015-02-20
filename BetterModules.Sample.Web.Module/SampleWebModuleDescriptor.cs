using BetterModules.Core.Web.Modules;

namespace BetterModules.Sample.Web.Module
{
    public class SampleWebModuleDescriptor : WebModuleDescriptor
    {
        public override string Description
        {
            get
            {
                return "Sample BetterModules Web Module";
            }
        }

        public override string Name
        {
            get
            {
                return "BetterModulesWebSample";
            }
        }
    }
}
