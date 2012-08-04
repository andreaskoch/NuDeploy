using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Configuration
{
    [TestFixture]
    public class BuildFolderPathProviderTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var result = new BuildFolderPathProvider(applicationInformation, filesystemAccessor);

            // Arrange
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ApplicationInformation applicationInformation = null;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new BuildFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new BuildFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        #endregion

        #region GetPackagingFolderPath

        [Test]
        public void GetBuildFolderPath_ResultEqulsFolderSpecifiedByTheApplicationInformationPrePackagingFolderProperty()
        {
            // Arrange
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

            var buildFolderPathProvider = new BuildFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            var result = buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.AreEqual(applicationInformation.BuildFolder, result);
        }

        [Test]
        public void GetBuildFolderPath_DirectoryExistsIsCalled()
        {
            // Arrange
            bool directoryExistsGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.BuildFolder)).Returns(
                () =>
                    {
                        directoryExistsGotCalled = true;
                        return true;
                    });

            var buildFolderPathProvider = new BuildFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.IsTrue(directoryExistsGotCalled);
        }

        [Test]
        public void GetBuildFolderPath_FolderIsCreatedIfItDoesNotExist()
        {
            // Arrange
            bool createDirectoryGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.BuildFolder)).Returns(false);
            filesystemAccessorMock.Setup(f => f.CreateDirectory(applicationInformation.BuildFolder)).Returns(
                () =>
                    {
                        createDirectoryGotCalled = true;
                        return true;
                    });

            var buildFolderPathProvider = new BuildFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.IsTrue(createDirectoryGotCalled);
        }

        [Test]
        public void GetBuildFolderPath_FolderExists_FolderIsDeletedAndThenCreated()
        {
            // Arrange
            bool deleteDirectoryGotCalled = false;
            bool createDirectoryGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.BuildFolder)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DeleteDirectory(applicationInformation.BuildFolder)).Returns(
                () =>
                {
                    deleteDirectoryGotCalled = true;
                    return true;
                });
            filesystemAccessorMock.Setup(f => f.CreateDirectory(applicationInformation.BuildFolder)).Returns(
                () =>
                {
                    createDirectoryGotCalled = true;
                    return true;
                });

            var buildFolderPathProvider = new BuildFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            buildFolderPathProvider.GetBuildFolderPath();

            // Assert
            Assert.IsTrue(deleteDirectoryGotCalled);
            Assert.IsTrue(createDirectoryGotCalled);
        }

        #endregion
    }
}