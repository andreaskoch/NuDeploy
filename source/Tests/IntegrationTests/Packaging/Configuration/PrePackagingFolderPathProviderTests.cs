using System;
using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Packaging.Configuration
{
    [TestFixture]
    public class PrePackagingFolderPathProviderTests
    {
        private ApplicationInformation applicationInformation;

        private IFilesystemAccessor fileSystemAccessor;

        private IPrePackagingFolderPathProvider packagingFolderPathProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            this.fileSystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            this.packagingFolderPathProvider = new PrePackagingFolderPathProvider(this.applicationInformation, this.fileSystemAccessor);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            string tempFolderPath = this.applicationInformation.PrePackagingFolder;

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }

        #region GetPrePackagingFolderPath

        [Test]
        public void GetPrePackagingFolderPath_FolderDoesNotExist_FolderIsCreated()
        {
            // Act
            var result = this.packagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.IsTrue(Directory.Exists(result));
        }

        [Test]
        public void GetPrePackagingFolderPath_FolderExists_FolderIsDeletedAndRecreated()
        {
            // Arrange
            var prePackagingFolderPath = this.packagingFolderPathProvider.GetPrePackagingFolderPath();

            string tempFile = Path.Combine(prePackagingFolderPath, "temp.txt");
            File.WriteAllText(tempFile, Guid.NewGuid().ToString());

            // Act
            var result = this.packagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.IsTrue(Directory.Exists(result));
            Assert.AreEqual(0, Directory.GetFiles(result, "*", SearchOption.AllDirectories).Length);
            Assert.IsFalse(File.Exists(tempFile));
        }

        #endregion
    }
}