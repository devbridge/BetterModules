using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Autofac;
using BetterModules.Core.Configuration;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Dependencies;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Modules;
using Moq;

namespace BetterModules.Core.Tests.TestHelpers
{
    public class LocalDatabase : IDisposable
    {
        private string tempFile;
        private string tempDir;
        private string basePath;

        private readonly string connectionString;

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

        public LocalDatabase()
        {
            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            tempDir = Path.Combine(basePath, "Temp");

            TryDeleteOldTemporaryFiles();

            var originalFile = Path.Combine(basePath, "BetterModulesTestsDataSet.mdf");

            tempFile = Path.Combine(tempDir, string.Format("BetterModulesTestsDataSet_{0}.mdf", Guid.NewGuid().ToString()));

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            File.Copy(originalFile, tempFile);

            connectionString = string.Format("Data Source=(LocalDb)\\v11.0; Initial Catalog=BetterModulesTestsDataSet; Integrated Security=SSPI; AttachDBFilename={0}", tempFile.TrimEnd('\\'));
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
            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var assemblyLoader = container.Resolve<IAssemblyLoader>();

                if (versionChecker == null)
                {
                    var mock = new Mock<IVersionChecker>();
                    mock
                        .Setup(vc => vc.VersionExists(It.IsAny<string>(), It.IsAny<long>()))
                        .Returns<string, long>((s, l) => false);
                    versionChecker = mock.Object;
                }

                var configuration = new Mock<IConfiguration>();
                configuration
                    .Setup(c => c.Database)
                    .Returns(() => new DatabaseConfigurationElement
                    {
                        ConnectionString = connectionString
                    });

                var migrationRunner = new DefaultMigrationRunner(assemblyLoader, configuration.Object, versionChecker);
                migrationRunner.MigrateStructure(descriptors);
            }
        }
    }
}
