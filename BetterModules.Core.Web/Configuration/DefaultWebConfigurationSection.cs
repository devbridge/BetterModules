using System;
using System.Configuration;
using BetterModules.Core.Configuration;

namespace BetterModules.Core.Web.Configuration
{
    public class DefaultWebConfigurationSection : DefaultConfigurationSection, IWebConfiguration
    {
        private const string WebSiteUrlAttribute = "webSiteUrl";

        /// <summary>
        /// Gets or sets the web site URL.
        /// </summary>
        /// <value>
        /// The web site URL.
        /// </value>
        [ConfigurationProperty(WebSiteUrlAttribute, DefaultValue = "Auto", IsRequired = false)]
        public string WebSiteUrl
        {
            get { return Convert.ToString(this[WebSiteUrlAttribute]); }
            set { this[WebSiteUrlAttribute] = value; }
        }
    }
}
