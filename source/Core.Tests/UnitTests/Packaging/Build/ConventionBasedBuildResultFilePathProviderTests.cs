using System;
using System.Collections.Generic;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Filesystem;
using NuDeploy.Core.Services.Packaging.Build;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Packaging.Build
{
    [TestFixture]
    public class ConventionBasedBuildResultFilePathProviderTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            // Act
            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Assert
            Assert.IsNotNull(buildResultFilePathProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            // Act
            new ConventionBasedBuildResultFilePathProvider(null, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildFolderPathProviderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            // Act
            new ConventionBasedBuildResultFilePathProvider(filesystemAccessor.Object, null, relativeFilePathInfoFactory.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_RelativeFilePathInfoFactoryParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();

            // Act
            new ConventionBasedBuildResultFilePathProvider(filesystemAccessor.Object, buildFolderPathProvider.Object, null);
        }

        #endregion

        #region GetWebsiteFilePaths

        [Test]
        public void GetWebsiteFilePaths_FolderDoesNotExist_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(false);
            
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebsiteFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetWebsiteFilePaths_FolderExist_ButIsEmpty_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(new List<DirectoryInfo>());

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebsiteFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetWebsiteFilePaths_FolderExist_ButContainsNoWebsites_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var publishedWebApplicationDirectories = new List<DirectoryInfo>
                {
                    new DirectoryInfo(Path.Combine(buildFolder, "WebApplication1")),
                    new DirectoryInfo(Path.Combine(buildFolder, "WebApplication2")),
                    new DirectoryInfo(Path.Combine(buildFolder, "WebApplication3"))
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(publishedWebApplicationDirectories);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebsiteFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetWebsiteFilePaths_FolderExist_ContainsOneWebsite_ResultCountEqualsFileCount_GetRelativeFilePathInfoIsCalledForEachFile()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            string websitePath = Path.Combine(
                buildFolder,
                ConventionBasedBuildResultFilePathProvider.FolderNamePublishedWebsites,
                string.Format("Test.{0}.1", ConventionBasedBuildResultFilePathProvider.WebsiteFolderFragmentIdentifier));

            var websiteFiles = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(websitePath, "file1.txt")),
                    new FileInfo(Path.Combine(websitePath, "file2.txt")),
                };

            var publishedWebApplicationDirectories = new List<DirectoryInfo>
                {
                    new DirectoryInfo(websitePath)
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(publishedWebApplicationDirectories);
            filesystemAccessor.Setup(f => f.GetAllFiles(websitePath)).Returns(websiteFiles);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebsiteFilePaths();

            // Assert
            Assert.AreEqual(websiteFiles.Count, result.Length);
            foreach (var websiteFile in websiteFiles)
            {
                string absolutePath = websiteFile.FullName;
                relativeFilePathInfoFactory.Verify(r => r.GetRelativeFilePathInfo(absolutePath, It.IsAny<string>()), Times.Once());
            }
        }

        #endregion

        #region GetWebApplicationFilePaths

        [Test]
        public void GetWebApplicationFilePaths_FolderDoesNotExist_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(false);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebApplicationFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetWebApplicationFilePaths_FolderExist_ButIsEmpty_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(new List<DirectoryInfo>());

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebApplicationFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetWebApplicationFilePaths_FolderExist_ContainsOneApplication_ResultCountEqualsFileCount_GetRelativeFilePathInfoIsCalledForEachFile()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            string webApplicationPath = Path.Combine(buildFolder, ConventionBasedBuildResultFilePathProvider.FolderNamePublishedWebsites, "Test.App.1");

            var files = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(webApplicationPath, "file1.txt")),
                    new FileInfo(Path.Combine(webApplicationPath, "file2.txt")),
                };

            var publishedWebApplicationDirectories = new List<DirectoryInfo>
                {
                    new DirectoryInfo(webApplicationPath)
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(publishedWebApplicationDirectories);
            filesystemAccessor.Setup(f => f.GetAllFiles(webApplicationPath)).Returns(files);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetWebApplicationFilePaths();

            // Assert
            Assert.AreEqual(files.Count, result.Length);
            foreach (var websiteFile in files)
            {
                string absolutePath = websiteFile.FullName;
                relativeFilePathInfoFactory.Verify(r => r.GetRelativeFilePathInfo(absolutePath, It.IsAny<string>()), Times.Once());
            }
        }

        #endregion

        #region GetApplicationFilePaths

        [Test]
        public void GetApplicationFilePaths_FolderDoesNotExist_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(false);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetApplicationFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetApplicationFilePaths_FolderExist_ButIsEmpty_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(new List<DirectoryInfo>());

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetApplicationFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetApplicationFilePaths_FolderExist_ContainsOneApplication_ResultCountEqualsFileCount_GetRelativeFilePathInfoIsCalledForEachFile()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            string applicationPath = Path.Combine(buildFolder, ConventionBasedBuildResultFilePathProvider.FolderNamePublishedApplications, "Test.App.1");

            var files = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(applicationPath, "file1.txt")),
                    new FileInfo(Path.Combine(applicationPath, "file2.txt")),
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(files);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetApplicationFilePaths();

            // Assert
            Assert.AreEqual(files.Count, result.Length);
            foreach (var websiteFile in files)
            {
                string absolutePath = websiteFile.FullName;
                relativeFilePathInfoFactory.Verify(r => r.GetRelativeFilePathInfo(absolutePath, It.IsAny<string>()), Times.Once());
            }
        }

        #endregion

        #region GetDeploymentPackageAdditionFilePaths

        [Test]
        public void GetDeploymentPackageAdditionFilePaths_FolderDoesNotExist_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(false);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetDeploymentPackageAdditionFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetDeploymentPackageAdditionFilePaths_FolderExist_ButIsEmpty_ResultIsEmptyList()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetSubDirectories(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(new List<DirectoryInfo>());

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetDeploymentPackageAdditionFilePaths();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetDeploymentPackageAdditionFilePaths_FolderExist_ContainsOneApplication_ResultCountEqualsFileCount_GetRelativeFilePathInfoIsCalledForEachFile()
        {
            // Arrange
            string buildFolder = Path.GetFullPath("build-folder");

            string applicationPath = Path.Combine(buildFolder, ConventionBasedBuildResultFilePathProvider.FolderNameDeploymentPackageAdditions, "Test.App.1");

            var files = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(applicationPath, "file1.txt")),
                    new FileInfo(Path.Combine(applicationPath, "file2.txt")),
                };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(true);
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(s => s.StartsWith(buildFolder)))).Returns(files);

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            buildFolderPathProvider.Setup(b => b.GetBuildFolderPath()).Returns(buildFolder);

            var relativeFilePathInfoFactory = new Mock<IRelativeFilePathInfoFactory>();

            var buildResultFilePathProvider = new ConventionBasedBuildResultFilePathProvider(
                filesystemAccessor.Object, buildFolderPathProvider.Object, relativeFilePathInfoFactory.Object);

            // Act
            var result = buildResultFilePathProvider.GetDeploymentPackageAdditionFilePaths();

            // Assert
            Assert.AreEqual(files.Count, result.Length);
            foreach (var websiteFile in files)
            {
                string absolutePath = websiteFile.FullName;
                relativeFilePathInfoFactory.Verify(r => r.GetRelativeFilePathInfo(absolutePath, It.IsAny<string>()), Times.Once());
            }
        }

        #endregion
    }
}