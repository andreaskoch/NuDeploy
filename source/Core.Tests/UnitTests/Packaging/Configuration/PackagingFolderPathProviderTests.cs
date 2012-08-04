using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Configuration
{
    [TestFixture]
    public class PackagingFolderPathProviderTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var result = new PackagingFolderPathProvider(applicationInformation, filesystemAccessor);

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
            new PackagingFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new PackagingFolderPathProvider(applicationInformation, filesystemAccessor);
        }

        #endregion

        #region GetPackagingFolderPath

        [Test]
        public void GetPackagingFolderPath_ResultEqulsFolderSpecifiedByTheApplicationInformationPackagingFolderProperty()
        {
            // Arrange
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(true);

            var packagingFolderPathProvider = new PackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            var result = packagingFolderPathProvider.GetPackagingFolderPath();

            // Assert
            Assert.AreEqual(applicationInformation.PackagingFolder, result);
        }

        [Test]
        public void GetPackagingFolderPath_DirectoryExistsIsCalled()
        {
            // Arrange
            bool directoryExistsGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.PackagingFolder)).Returns(
                () =>
                    {
                        directoryExistsGotCalled = true;
                        return true;
                    });

            var packagingFolderPathProvider = new PackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            packagingFolderPathProvider.GetPackagingFolderPath();

            // Assert
            Assert.IsTrue(directoryExistsGotCalled);
        }

        [Test]
        public void GetPackagingFolderPath_FolderIsCreatedIfItDoesNotExist()
        {
            // Arrange
            bool createDirectoryGotCalled = false;

            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(applicationInformation.PackagingFolder)).Returns(false);
            filesystemAccessorMock.Setup(f => f.CreateDirectory(applicationInformation.PackagingFolder)).Returns(
                () =>
                    {
                        createDirectoryGotCalled = true;
                        return true;
                    });

            var packagingFolderPathProvider = new PackagingFolderPathProvider(applicationInformation, filesystemAccessorMock.Object);

            // Act
            packagingFolderPathProvider.GetPackagingFolderPath();

            // Assert
            Assert.IsTrue(createDirectoryGotCalled);
        }

        #endregion
    }
}