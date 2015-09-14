using BetterModules.Core.Models;
using FluentMigrator.VersionTableInfo;

namespace BetterModules.Sample.Module.Models.Migrations
{
    [VersionTableMetaData]
    public class RootVersionTableMetaData : IVersionTableMetaData
    {
        public object ApplicationContext { get; set; }

        public bool OwnsSchema { get; }

        public string SchemaName => SchemaNameProvider.GetSchemaName(SampleModuleDescriptor.ModuleName);

        public string TableName => "VersionInfo";

        public string ColumnName => "Version";

        public string DescriptionColumnName => "Description";

        public string UniqueIndexName => "uc_VersionInfo_Verion_" + SampleModuleDescriptor.ModuleName;

        public string AppliedOnColumnName => "AppliedOn";
    }
}