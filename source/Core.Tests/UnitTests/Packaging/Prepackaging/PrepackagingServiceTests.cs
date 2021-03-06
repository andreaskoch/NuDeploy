﻿using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Filesystem;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Configuration;
using NuDeploy.Core.Services.Packaging.PrePackaging;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Prepackaging
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
        public void Prepackage_PrepackagingFolderIsInvalid_FailureResultIsReturned(string prepackagingFolder)
        {
            // Arrange
            string buildFolder = "C:\\build";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(false);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Prepackage_PrepackagingFolderIsDoesNotExist_FailureResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Non-Existing-Folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Prepackage_OneOfTheSubFunctionsThrowsAnExecption_FailureResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();

            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);
            buildResultFilePathProvider.Setup(b => b.GetDeploymentPackageAdditionFilePaths(It.IsAny<string>())).Throws(new Exception());

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Prepackage_NoFilesInBuildResultFolder_NothingIsCopied_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
            filesystemAccessor.Verify(f => f.CopyFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Prepackage_DeploymentPackageAdditions_FilesAreCopied_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetDeploymentPackageAdditionFilePaths(It.IsAny<string>())).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = relativeFilePathInfo.RelativeFilePath;
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_AssemblyResourcesAreDownloaded_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
            assemblyResourceDownloader.Verify(d => d.Download(prepackagingFolder), Times.Once());
        }

        [Test]
        public void Prepackage_Websites_FilesAreCopiedToTheWebsitesFolder_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetWebsiteFilePaths(It.IsAny<string>())).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = Path.Combine(PrepackagingService.TargetFolderNameWebsites, relativeFilePathInfo.RelativeFilePath);
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_WebApplications_FilesAreCopiedToTheWebApplicationsFolder_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetWebApplicationFilePaths(It.IsAny<string>())).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
            foreach (var relativeFilePathInfo in relativeFilePathInfos)
            {
                string absolutePath = relativeFilePathInfo.AbsoluteFilePath;
                string relativePath = Path.Combine(PrepackagingService.TargetFolderNameWebApplications, relativeFilePathInfo.RelativeFilePath);
                filesystemAccessor.Verify(f => f.CopyFile(absolutePath, It.Is<string>(s => s.EndsWith(relativePath))), Times.Once());
            }
        }

        [Test]
        public void Prepackage_Applications_FilesAreCopiedToTheApplicationsFolder_SuccessResultIsReturned()
        {
            // Arrange
            string buildFolder = "C:\\build";
            string prepackagingFolder = Path.GetFullPath("Prepackaging");

            var relativeFilePathInfos = new[]
                {
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file1.txt", @"some-folder\some-file1.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-folder\some-file2.txt", @"some-folder\some-file2.txt"),
                    new RelativeFilePathInfo(@"C:\app\some-other-folder\some-file1.txt", @"some-other-folder\some-file1.txt")
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(prepackagingFolder)).Returns(true);
            filesystemAccessor.Setup(f => f.DirectoryExists(buildFolder)).Returns(true);

            var assemblyResourceDownloader = new Mock<IAssemblyResourceDownloader>();
            var buildResultFilePathProvider = new Mock<IBuildResultFilePathProvider>();
            buildResultFilePathProvider.Setup(b => b.GetApplicationFilePaths(It.IsAny<string>())).Returns(relativeFilePathInfos);

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(prepackagingFolder);

            var prepackagingService = new PrepackagingService(
                filesystemAccessor.Object, assemblyResourceDownloader.Object, buildResultFilePathProvider.Object, prePackagingFolderPathProvider.Object);

            // Act
            var result = prepackagingService.Prepackage(buildFolder);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
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