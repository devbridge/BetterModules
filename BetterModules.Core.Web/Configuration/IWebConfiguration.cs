using BetterModules.Core.Configuration;

namespace BetterModules.Core.Web.Configuration
{
    public interface IWebConfiguration : IConfiguration
    {
        /// <summary>
        /// Gets or sets the web site URL.
        /// </summary>
        /// <value>
        /// The web site URL.
        /// </value>
        string WebSiteUrl { get; set; }
    }
}
