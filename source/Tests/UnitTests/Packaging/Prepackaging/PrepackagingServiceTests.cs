using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Filesystem;
using NuDeploy.Core.Services.Packaging.Configuration;
using NuDeploy.Core.Services.Packaging.PrePackaging;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Packaging.Prepackaging
{
    [TestFixture]
    public class PrepackagingServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            // Act
            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Assert
            Assert.IsNotNull(prepackagingService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            // Act
            new PrepackagingService(null, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_AssemblyResourceDownloaderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            // Act
            new PrepackagingService(
                filesystemAccessor.Object, null, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildResultFilePathProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            // Act
            new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, null, prePackagingFolderPathProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PrePackagingFolderPathProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();

            // Act
            new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, null);
        }

        #endregion

        #region Prepackage

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Prepackage_BuildConfigurationIsNotValid_ArgumentExceptionIsThrown(string buildConfiguration)
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            prepackagingService.Prepackage(buildConfiguration);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Prepackage_PrepackagingFolderIsInvalid_ResultIsFalse(string prepackagingFolder)
        {
            // Arrange
            string buildConfiguration = "Debug";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(false);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Prepackage_CopyFilesToPrePackagingFolderThrowsAnException_ResultIsFalse()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();

            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetNuspecFilePath(It.IsAny<string>())).Throws(new Exception());

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Prepackage_PrepackagingFolderIsDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Non-Existing-Folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Prepackage_NoFilesInBuildResultFolder_NothingIsCopied_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            filesystemAccessor.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Prepackage_NuspecFileExists_FileIsCopied_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var nuspecFile = new RelativeFilePathInfo(@"C:\app\some-folder\some-file.txt", @"some-folder\some-file.txt");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetNuspecFilePath(buildConfiguration)).Returns(nuspecFile);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            filesystemAccessor.Verify(f => f.CopyFile(nuspecFile.AbsoluteFilePath, It.Is<string>(s => s.EndsWith(nuspecFile.RelativeFilePath))), Times.Once());
        }

        [Test]
        public void Prepackage_DeploymentPackageAdditions_FilesAreCopied_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetDeploymentPackageAdditionFilePaths()).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = relativeFilePathInfo.RelativeFilePath;
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_AssemblyResourcesAreDownloaded_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            assemblyResourceDownloader.Verify(d => d.Download(prepackagingFolder), Times.Once());
        }

        [Test]
        public void Prepackage_Websites_FilesAreCopiedToTheWebsitesFolder_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetWebsiteFilePaths()).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = Path.Combine(PrepackagingService.TargetFolderNameWebsites, relativeFilePathInfo.RelativeFilePath);
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_WebApplications_FilesAreCopiedToTheWebApplicationsFolder_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetWebApplicationFilePaths()).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = Path.Combine(PrepackagingService.TargetFolderNameWebApplications, relativeFilePathInfo.RelativeFilePath);
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_Applications_FilesAreCopiedToTheApplicationsFolder_ResultIsTrue()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetApplicationFilePaths()).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildConfiguration);

            // Assert
            Assert.IsTrue(result);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = Path.Combine(PrepackagingService.TargetFolderNameApplications, relativeFilePathInfo.RelativeFilePath);
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        #endregion
    }
}