using System;
using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Packaging.Configuration
{
    [TestFixture]
    public class BuildFolderPathProviderTests
    {
        private ApplicationInformation applicationInformation;

        private IFilesystemAccessor fileSystemAccessor;

        private IBuildFolderPathProvider buildFolderPathProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            this.fileSystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            this.buildFolderPathProvider = new BuildFolderPathProvider(this.applicationInformation, this.fileSystemAccessor);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            string tempFolderPath = this.applicationInformation.BuildFolder;

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }

        #region GetBuildFolderPath

        [Test]
        public void GetBuildFolderPath_FolderDoesNotExist_FolderIsCreated()
        {
            // Act
            var result = this.buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.IsTrue(Directory.Exists(result));
        }

        [Test]
        public void GetBuildFolderPath_FolderExists_FolderIsDeletedAndRecreated()
        {
            // Arrange
            var buildFolderPath = this.buildFolderPathProvider.GetBuildFolderPath();

            string tempFile = Path.Combine(buildFolderPath, "temp.txt");
            File.WriteAllText(tempFile, Guid.NewGuid().ToString());

            // Act
            var result = this.buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.IsTrue(Directory.Exists(result));
            Assert.AreEqual(0, Directory.GetFiles(result, "*", SearchOption.AllDirectories).Length);
            Assert.IsFalse(File.Exists(tempFile));
        }

        #endregion
    }
}