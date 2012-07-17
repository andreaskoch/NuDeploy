using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Update;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Update
{
    [TestFixture]
    public class SelfUpdateServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Assert
            Assert.IsNotNull(selfUpdateService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new SelfUpdateService(null, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new SelfUpdateService(userInterface.Object, null, packageRepositoryBrowser.Object, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryBrowserParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new SelfUpdateService(userInterface.Object, applicationInformation, null, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();

            // Act
            new SelfUpdateService(userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, null);
        }

        #endregion

        #region SelfUpdate

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void SelfUpdate_ExePathIsInvalid_ArgumentExceptionIsThrown(string exePath)
        {
            // Arrange
            var version = new Version(1, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            selfUpdateService.SelfUpdate(exePath, version);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SelfUpdate_VersionIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            Version version = null;

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            selfUpdateService.SelfUpdate(exePath, version);
        }

        [Test]
        public void SelfUpdate_PackageIsNotFound_ResultIsFalse()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsOlderThanCurrentVersion_ResultIsFalse()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(2, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(1, 0, 0, 0));

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsNewerThanTheCurrentVersion_PackageContainsNoExeWhichNameMatchesTheApplication_ResultIsFalse()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation { NameOfExecutable = exePath };
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packageFileMock = new Mock<IPackageFile>();
            packageFileMock.Setup(f => f.Path).Returns("tools\\" + "SomeOtherApplicationName.exe");

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(2, 0, 0, 0));
            packageMock.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { packageFileMock.Object });

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsNewerThanTheCurrentVersion_PackageContainsExeWhichNameMatchesTheApplication_CurrentExeIsMovedToNewLocation()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation { NameOfExecutable = exePath };
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packageFileMock = new Mock<IPackageFile>();
            packageFileMock.Setup(f => f.Path).Returns("tools\\" + exePath);

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(2, 0, 0, 0));
            packageMock.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { packageFileMock.Object });

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            filesystemAccessor.Verify(
                f => f.MoveFile(exePath, It.Is<string>(s => s.Equals(exePath, StringComparison.OrdinalIgnoreCase) == false)), Times.Once());
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsNewerThanTheCurrentVersion_PackageContainsExeWhichNameMatchesTheApplication_PackageSourceStreamIsNull_ResultIsFalse()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation { NameOfExecutable = exePath };
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            Stream sourceStream = null;
            var packageFileMock = new Mock<IPackageFile>();
            packageFileMock.Setup(f => f.Path).Returns("tools\\" + exePath);
            packageFileMock.Setup(f => f.GetStream()).Returns(sourceStream);

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(2, 0, 0, 0));
            packageMock.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { packageFileMock.Object });

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsNewerThanTheCurrentVersion_PackageContainsExeWhichNameMatchesTheApplication_UpdateTargetStreamIsNull_ResultIsFalse()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation { NameOfExecutable = exePath };
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packageContent = "Content of the update package";
            var packageContentStream = TestUtilities.GetStreamReaderForText(packageContent).BaseStream;

            var packageFileMock = new Mock<IPackageFile>();
            packageFileMock.Setup(f => f.Path).Returns("tools\\" + exePath);
            packageFileMock.Setup(f => f.GetStream()).Returns(packageContentStream);

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(2, 0, 0, 0));
            packageMock.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { packageFileMock.Object });

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            Stream targetStream = null;
            filesystemAccessor.Setup(f => f.GetWriteStream(exePath)).Returns(targetStream);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SelfUpdate_PackageIsFound_VersionIsNewerThanTheCurrentVersion_PackageContainsExeWhichNameMatchesTheApplication_PackageContentIsWrittenToExecutable()
        {
            // Arrange
            string exePath = "NuDeploy.exe";
            var version = new Version(1, 0, 0, 0);

            var userInterface = new Mock<IUserInterface>();
            var applicationInformation = new ApplicationInformation { NameOfExecutable = exePath };
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packageContent = "Content of the update package";
            var packageContentStream = TestUtilities.GetStreamReaderForText(packageContent).BaseStream;

            var packageFileMock = new Mock<IPackageFile>();
            packageFileMock.Setup(f => f.Path).Returns("tools\\" + exePath);
            packageFileMock.Setup(f => f.GetStream()).Returns(packageContentStream);

            var packageMock = new Mock<IPackage>();
            packageMock.Setup(p => p.Version).Returns(new SemanticVersion(2, 0, 0, 0));
            packageMock.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { packageFileMock.Object });

            var targetStream = new MemoryStream();
            filesystemAccessor.Setup(f => f.GetWriteStream(exePath)).Returns(targetStream);

            IPackageRepository packageRepository;
            packageRepositoryBrowser.Setup(p => p.FindPackage(It.IsAny<string>(), out packageRepository)).Returns(packageMock.Object);

            var selfUpdateService = new SelfUpdateService(
                userInterface.Object, applicationInformation, packageRepositoryBrowser.Object, filesystemAccessor.Object);

            // Act
            bool result = selfUpdateService.SelfUpdate(exePath, version);

            // Assert
            var bytes = targetStream.ReadAllBytes();
            string newContentOfOldFile = Encoding.UTF8.GetString(bytes);
            filesystemAccessor.Verify(f => f.GetWriteStream(exePath), Times.Once());
            Assert.AreEqual(packageContent, newContentOfOldFile);
            Assert.IsTrue(result);
        }

        #endregion
    }
}