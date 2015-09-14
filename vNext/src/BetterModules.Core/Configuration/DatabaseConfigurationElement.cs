namespace BetterModules.Core.Configuration
{
    public class DatabaseConfigurationElement: IDatabaseConfiguration
    {
        public string SchemaName { get; set; }

        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }

        public string ConnectionProvider { get; set; }

        public DatabaseType DatabaseType { get; set; }
    }
}