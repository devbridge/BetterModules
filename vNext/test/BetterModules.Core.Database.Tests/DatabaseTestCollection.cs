using Xunit;

namespace BetterModules.Core.Database.Tests
{
    [CollectionDefinition("Database test collection")]
    public class DatabaseTestCollection: ICollectionFixture<DatabaseTestFixture>
    {
         
    }
}