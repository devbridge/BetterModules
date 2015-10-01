using BetterModules.Core.Models;
using Xunit;

namespace BetterModules.Core.Tests.Models
{
    public class SchemaNameProviderTests
    {
        [Fact]
        public void Should_Provide_Correct_Schema_Name()
        {
            var origPattern = SchemaNameProvider.SchemaNamePattern;

            try
            {
                SchemaNameProvider.SchemaNamePattern = "test_{0}_test";

                Assert.Equal(SchemaNameProvider.GetSchemaName("MoDuLe"), "test_module_test");
            }
            finally
            {
                SchemaNameProvider.SchemaNamePattern = origPattern;
            }
        }
    }
}
