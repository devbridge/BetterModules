using System.IO;

using Autofac;

using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Environment.FileSystem;
using BetterModules.Core.Modules.Registration;

using BetterModules.Sample.Module;

using NUnit.Framework;

namespace BetterModules.Core.Database.Tests.DataAccess.DataContext.Migrations
{
    [TestFixture]
    public class DefaultVersionCheckerTests : DatabaseTestBase
    {
        [Test]
        public void Should_Check_In_The_Database()
        {
            var checker = GetVersionCheckerImplementation();

            // Delete file, so data will be taken from the database
            if (File.Exists(checker.CacheFilePath))
            {
                File.Delete(checker.CacheFilePath);
            }

            Assert.IsTrue(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181430));
            Assert.IsTrue(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181440));

            Assert.IsFalse(checker.VersionExists(SampleModuleDescriptor.ModuleName, 123456));
            Assert.IsFalse(checker.VersionExists("Tests", 201502181430));
        }

        [Test]
        public void Should_Check_In_The_File_System()
        {
            var checker = GetVersionCheckerImplementation();

            // Create file if not created
            if (!File.Exists(checker.CacheFilePath))
            {
                File.WriteAllText(checker.CacheFilePath, "201502181430 BetterModulesSample\\r\\n201502181440 BetterModulesSample");
            }

            Assert.IsTrue(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181430));
            Assert.IsTrue(checker.VersionExists(SampleModuleDescriptor.ModuleName, 201502181440));

            Assert.IsFalse(checker.VersionExists(SampleModuleDescriptor.ModuleName, 123456));
            Assert.IsFalse(checker.VersionExists("Tests", 201502181430));
        }

        [Test]
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

            Assert.IsFalse(checker.VersionExists("TestModule", 123456));
            checker.AddVersion("TestModule", 123456);
            Assert.IsTrue(checker.VersionExists("TestModule", 123456));

            // Cache file should be recreated and new version should be added
            Assert.IsTrue(File.Exists(checker.CacheFilePath));

            var fileSource = File.ReadAllText(checker.CacheFilePath).Trim();
            Assert.AreEqual(fileSource, "123456 TestModule");

            // Restore file
            if (File.Exists(checker.CacheFilePath))
            {
                File.Delete(checker.CacheFilePath);
            }
            File.Move(tempFile, checker.CacheFilePath);
        }

        private DefaultVersionChecker GetVersionCheckerImplementation()
        {
            var modulesRegistration = Container.Resolve<IModulesRegistration>();
            var workingDirectory = Container.Resolve<IWorkingDirectory>();

            var versionChecker = new DefaultVersionChecker(UnitOfWork, modulesRegistration, workingDirectory);

            return versionChecker;
        }
    }
}
