using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Database.Tests.TestHelpers.Migrations;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Moq;

namespace BetterModules.Core.Database.Tests.TestHelpers
{
    public class LocalDatabase : IDisposable
    {
        private string tempFile;
        private string tempDir;
        private string basePath;

        private readonly string connectionString;
        private readonly IServiceProvider serviceProvider;

        private SqlConnection sqlConnection;

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public SqlConnection SqlConnection
        {
            get
            {
                if (sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                }
                return sqlConnection;
            }
        }

        public LocalDatabase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            basePath = Path.Combine(System.Environment.CurrentDirectory, "App_Data");
            tempDir = Path.Combine(basePath, "Temp");

            TryDeleteOldTemporaryFiles();

            var originalFile = Path.Combine(basePath, "BetterModulesTestsDataSet.mdf");

            tempFile = Path.Combine(tempDir, $"BetterModulesTestsDataSet_{Guid.NewGuid().ToString()}.mdf");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            File.Copy(originalFile, tempFile);

            connectionString =
                $"Data Source=(LocalDb)\\v11.0; Initial Catalog=BetterModulesTestsDataSet; Integrated Security=SSPI; AttachDBFilename={tempFile.TrimEnd('\\')}";
        }

        public void Dispose()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            DeleteTemporaryDatabase();
        }

        private void TryDeleteOldTemporaryFiles()
        {
            if (Directory.Exists(tempDir))
            {
                var files = Directory.GetFiles(tempDir);
                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file);
                    var fileName = Path.GetFileName(file);
                    if ((ext.ToLowerInvariant().EndsWith("mdf") || ext.ToLowerInvariant().EndsWith("ldf")) && fileName.StartsWith("BetterModulesTestsDataSet_"))
                    {
                        DeleteTemporaryDatabase(file);
                    }
                }
            }
        }

        private void DeleteTemporaryDatabase(string file = null)
        {
            try
            {
                if (file == null)
                {
                    file = tempFile;
                }
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // DO nothing - database file may be locked
            }
        }

        public void RunMigrations(List<ModuleDescriptor> descriptors, IVersionChecker versionChecker = null)
        {
                var assemblyLoader = serviceProvider.GetService<IAssemblyLoader>();

                if (versionChecker == null)
                {
                    var mock = new Mock<IVersionChecker>();
                    mock
                        .Setup(vc => vc.VersionExists(It.IsAny<string>(), It.IsAny<long>()))
                        .Returns<string, long>((s, l) => false);
                    versionChecker = mock.Object;
                }

                var configuration = new Mock<IOptions<DefaultConfigurationSection>>();
                configuration
                    .Setup(c => c.Value)
                    .Returns(() => new DefaultConfigurationSection
                    {
                        Database = new DatabaseConfigurationElement
                        {
                            ConnectionString = connectionString
                        }
                    });

                var migrationRunner = new DefaultMigrationRunner(assemblyLoader, configuration.Object, versionChecker, new LoggerFactory());
                migrationRunner.MigrateStructure(descriptors);
        }
    }
}
