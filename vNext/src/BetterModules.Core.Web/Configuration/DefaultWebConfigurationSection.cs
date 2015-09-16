using BetterModules.Core.Configuration;

namespace BetterModules.Core.Web.Configuration
{
    public class DefaultWebConfigurationSection : DefaultConfigurationSection, IWebConfiguration
    {
        public string WebSiteUrl { get; set; }
    }
}
