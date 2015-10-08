using System.IO;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;
using BetterModules.Sample.Module;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Xunit;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Migrations
{
    [Collection("Database test collection")]
    public class DefaultVersionCheckerTests
    {
        private readonly IUnitOfWork unitOfWork;
        private DatabaseTestFixture fixture;

        public DefaultVersionCheckerTests(DatabaseTestFixture fixture)
        {
            var provider = fixture.Services.BuildServiceProvider();
            unitOfWork = provider.GetService<IUnitOfWork>();
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Check_In_The_Database()
        {
            var checker = GetVersionCheckerImplementation();

            // Delete file, so data will be taken from the database
            if (File.Exists(checker.CacheFilePath))
            {
                File.Delete(checker.CacheFilePath);
            }

            Assert.True(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181430));
            Assert.True(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181440));

            Assert.False(checker.VersionExists(SampleModuleDescriptor.ModuleName, 123456));
            Assert.False(checker.VersionExists("Tests", 201502181430));
        }

        [Fact]
        public void Should_Check_In_The_File_System()
        {
            var checker = GetVersionCheckerImplementation();

            // Create file if not created
            if (!File.Exists(checker.CacheFilePath))
            {
                File.WriteAllText(checker.CacheFilePath, "201502181430 BetterModulesSample\\r\\n201502181440 BetterModulesSample");
            }

            Assert.True(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181430));
            Assert.True(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181440));

            Assert.False(checker.VersionExists(SampleModuleDescriptor.ModuleName, 123456));
            Assert.False(checker.VersionExists("Tests", 201502181430));
        }

        [Fact]
        public void Should_Add_New_Version()
        {
            var checker = GetVersionCheckerImplementation();
            var tempFile = checker.CacheFilePath + ".temp";

            // Delete file, created from the previous migrations
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            if (File.Exists(checker.CacheFilePath))
            {
                File.Move(checker.CacheFilePath, tempFile);
            }

            Assert.False(checker.VersionExists("TestModule", 123456));
            checker.AddVersion("TestModule", 123456);
            Assert.True(checker.VersionExists("TestModule", 123456));

            // Cache file should be recreated and new version should be added
            Assert.True(File.Exists(checker.CacheFilePath));

            var fileSource = File.ReadAllText(checker.CacheFilePath).Trim();
            Assert.Equal(fileSource, "123456 TestModule");

            // Restore file
            if (File.Exists(checker.CacheFilePath))
            {
                File.Delete(checker.CacheFilePath);
            }
            File.Move(tempFile, checker.CacheFilePath);
        }

        private DefaultVersionChecker GetVersionCheckerImplementation()
        {
            var modulesRegistration = fixture.Provider.GetService<IModulesRegistration>();
            var workingDirectory = fixture.Provider.GetService<IWorkingDirectory>();

            var versionChecker = new DefaultVersionChecker(unitOfWork, modulesRegistration, workingDirectory, new LoggerFactory());

            return versionChecker;
        }
    }
}
