using System.Web.Configuration;
using BetterModules.Core.Configuration;

namespace BetterModules.Core.Web.Configuration
{
    public class DefaultWebConfigurationLoader : DefaultConfigurationLoader
    {
        protected override System.Configuration.Configuration OpenApplicationConfiguration()
        {
            return WebConfigurationManager.OpenWebConfiguration("~/Web.config");
        }
    }
}
