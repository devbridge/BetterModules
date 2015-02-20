using System.Configuration;

namespace BetterModules.Core.Configuration
{
    public interface IConfigurationLoader
    {
        T LoadConfig<T>() where T : ConfigurationSection;
        
        T TryLoadConfig<T>() where T : ConfigurationSection;
    }
}