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

using BetterModules.Sample.Module;

using Moq;

using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext.Migrations
{
    [TestFixture]
    public class DefaultMigrationRunnerIntegrationTests : TestBase
    {
        private string tempFile;
        private string tempDir;
        private string basePath;

        [Test]
        public void Should_Run_Sample_Module_Migration_Successfully()
        {
            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            tempDir = Path.Combine(basePath, "Temp");

            using (var container = ContextScopeProvider.CreateChildContainer())
            {
                var connectionString = PrepareTemporaryDatabase();

                var versionUpdateCount = 0;
                var assemblyLoader = container.Resolve<IAssemblyLoader>();
                var versionChecker = new Mock<IVersionChecker>();
                versionChecker
                    .Setup(vc => vc.VersionExists(It.IsAny<string>(), It.IsAny<long>()))
                    .Returns<string, long>((s, l) => false);
                versionChecker
                    .Setup(vc => vc.AddVersion(It.IsAny<string>(), It.IsAny<long>()))
                    .Callback<string, long>((s, l) => versionUpdateCount++);

                var configuration = new Mock<IConfiguration>();
                configuration
                    .Setup(c => c.Database)
                    .Returns(() => new DatabaseConfigurationElement
                    {
                        ConnectionString = connectionString
                    });

                var migrationRunner = new DefaultMigrationRunner(assemblyLoader, configuration.Object, versionChecker.Object);
                var descriptors = new List<ModuleDescriptor> { new SampleModuleDescriptor() };

                migrationRunner.MigrateStructure(descriptors);

                // Should run 2 migration scripts
                Assert.AreEqual(versionUpdateCount, 2);

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check, if tables are created
                    CheckIfTablesExist(connection);

                    connection.Close();
                }

                // Shouldn't run the same migrations second time
                versionUpdateCount = 0;
                versionChecker
                    .Setup(vc => vc.VersionExists(It.IsAny<string>(), It.IsAny<long>()))
                    .Returns<string, long>((s, l) => true);

                migrationRunner.MigrateStructure(descriptors);

                // Should run 0 migration scripts
                Assert.AreEqual(versionUpdateCount, 0);

                DeleteTemporaryDatabase(tempFile);
            }
        }

        private string PrepareTemporaryDatabase()
        {
            TryDeleteOldTemporaryFiles();

            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            var originalFile = Path.Combine(basePath, "BetterModulesTestsDataSet.mdf");

            tempDir = Path.Combine(basePath, "Temp");
            tempFile = Path.Combine(tempDir, string.Format("BetterModulesTestsDataSet_{0}.mdf", Guid.NewGuid().ToString()));

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            File.Copy(originalFile, tempFile);

            var connectionString = string.Format("Data Source=(LocalDb)\\v11.0; Initial Catalog=BetterModulesTestsDataSet; Integrated Security=SSPI; AttachDBFilename={0}", tempFile.TrimEnd('\\'));

            return connectionString;
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

        private void DeleteTemporaryDatabase(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
                // DO nothing - database file may be locked
            }
        }

        private void CheckIfTablesExist(SqlConnection connection)
        {
            var command = new SqlCommand("SELECT TABLE_NAME FROM information_schema.tables", connection);
            var tables = new List<string>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var table = reader.GetString(0);
                        tables.Add(table);
                    }
                    reader.Close();
                }
            }

            Assert.AreEqual(tables.Count, 3);
            Assert.IsTrue(tables.Contains("VersionInfo"));
            Assert.IsTrue(tables.Contains("TestTable"));
            Assert.IsTrue(tables.Contains("TestTable2"));
        }
    }
}
