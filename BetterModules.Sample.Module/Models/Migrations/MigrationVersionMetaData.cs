using BetterModules.Core.Models;
using FluentMigrator.VersionTableInfo;

namespace BetterModules.Sample.Module.Models.Migrations
{
    [VersionTableMetaData]
    public class RootVersionTableMetaData : IVersionTableMetaData
    {
        public object ApplicationContext { get; set; }

        public bool OwnsSchema
        {
            get { return true; }
        }

        public string SchemaName
        {
            get
            {
                return SchemaNameProvider.GetSchemaName(SampleModuleDescriptor.ModuleName);
            }
        }

        public string TableName
        {
            get
            {
                return "VersionInfo";
            }
        }

        public string ColumnName
        {
            get
            {
                return "Version";
            }
        }

        public string DescriptionColumnName
        {
            get { return "Description"; }
        }

        public string UniqueIndexName
        {
            get
            {
                return "uc_VersionInfo_Verion_" + SampleModuleDescriptor.ModuleName;
            }
        }

        public string AppliedOnColumnName
        {
            get { return "ApploedOn"; }
        }
    }
}