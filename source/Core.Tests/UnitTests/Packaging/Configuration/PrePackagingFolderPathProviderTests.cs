using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Configuration
{
    [TestFixture]
    public class PrePackagingFolderPathProviderTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var result = new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessor);

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
            new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        #endregion

        #region GetPackagingFolderPath

        [Test]
        public void GetPrePackagingFolderPath_ResultEqulsFolderSpecifiedByTheApplicationInformationPrePackagingFolderProperty()
        {
            // Arrange
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

            var prePackagingFolderPathProvider = new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            var result = prePackagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.AreEqual(applicationInformation.PrePackagingFolder, result);
        }

        [Test]
        public void GetPrePackagingFolderPath_DirectoryExistsIsCalled()
        {
            // Arrange
            bool directoryExistsGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.PrePackagingFolder)).Returns(
                () =>
                    {
                        directoryExistsGotCalled = true;
                        return true;
                    });

            var prePackagingFolderPathProvider = new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            prePackagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.IsTrue(directoryExistsGotCalled);
        }

        [Test]
        public void GetPrePackagingFolderPath_FolderIsCreatedIfItDoesNotExist()
        {
            // Arrange
            bool createDirectoryGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.PrePackagingFolder)).Returns(false);
            filesystemAccessorMock.Setup(f => f.CreateDirectory(applicationInformation.PrePackagingFolder)).Returns(
                () =>
                    {
                        createDirectoryGotCalled = true;
                        return true;
                    });

            var prePackagingFolderPathProvider = new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            prePackagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.IsTrue(createDirectoryGotCalled);
        }

        [Test]
        public void GetPrePackagingFolderPath_FolderExists_FolderIsDeletedAndThenCreated()
        {
            // Arrange
            bool deleteDirectoryGotCalled = false;
            bool createDirectoryGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.PrePackagingFolder)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DeleteDirectory(applicationInformation.PrePackagingFolder)).Returns(
                () =>
                {
                    deleteDirectoryGotCalled = true;
                    return true;
                });
            filesystemAccessorMock.Setup(f => f.CreateDirectory(applicationInformation.PrePackagingFolder)).Returns(
                () =>
                {
                    createDirectoryGotCalled = true;
                    return true;
                });

            var prePackagingFolderPathProvider = new PrePackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            prePackagingFolderPathProvider.GetPrePackagingFolderPath();

            // Assert
            Assert.IsTrue(deleteDirectoryGotCalled);
            Assert.IsTrue(createDirectoryGotCalled);
        }

        #endregion
    }
}