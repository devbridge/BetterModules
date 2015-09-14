namespace BetterModules.Core.Configuration
{
    public class DefaultConfigurationSection: IConfiguration
    {
        public DatabaseConfigurationElement Database { get; set; }

        IDatabaseConfiguration IConfiguration.Database => Database;
    }
}