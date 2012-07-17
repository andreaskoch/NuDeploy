using System;
using System.Collections.Generic;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Packaging.Configuration;
using NuDeploy.Core.Services.Packaging.Nuget;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Packaging.Nuget
{
    [TestFixture]
    public class PackagingServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreValid_ObjectIsInstantiated()
        {
            // Arrange
            IPrePackagingFolderPathProvider prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>().Object;
            IPackagingFolderPathProvider packagingFolderPathProvider = new Mock<IPackagingFolderPathProvider>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var packagingService = new PackagingService(prePackagingFolderPathProvider, packagingFolderPathProvider, filesystemAccessor);

            // Assert
            Assert.IsNotNull(packagingService);
        }

        [Test]
        public void Constructor_AllParametersAreValid_GetPrePackagingFolderPathGetsCalled()
        {
            // Arrange
            bool getPrePackagingFolderPathGotCalled = false;

            var prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProvider.Setup(p => p.GetPrePackagingFolderPath()).Returns(
                () =>
                    {
                        getPrePackagingFolderPathGotCalled = true;
                        return null;
                    });

            IPackagingFolderPathProvider packagingFolderPathProvider = new Mock<IPackagingFolderPathProvider>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new PackagingService(prePackagingFolderPathProvider.Object, packagingFolderPathProvider, filesystemAccessor);

            // Assert
            Assert.IsTrue(getPrePackagingFolderPathGotCalled);
        }

        [Test]
        public void Constructor_AllParametersAreValid_GetPackagingFolderPathGetsCalled()
        {
            // Arrange
            bool getPackagingFolderPathGotCalled = false;

            IPrePackagingFolderPathProvider prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>().Object;

            var packagingFolderPathProvider = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProvider.Setup(p => p.GetPackagingFolderPath()).Returns(
                () =>
                {
                    getPackagingFolderPathGotCalled = true;
                    return null;
                });

            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new PackagingService(prePackagingFolderPathProvider, packagingFolderPathProvider.Object, filesystemAccessor);

            // Assert
            Assert.IsTrue(getPackagingFolderPathGotCalled);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PrePackagingFolderPathProviderParametersIsInvalid_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IPrePackagingFolderPathProvider prePackagingFolderPathProvider = null;
            IPackagingFolderPathProvider packagingFolderPathProvider = new Mock<IPackagingFolderPathProvider>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new PackagingService(prePackagingFolderPathProvider, packagingFolderPathProvider, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackagingFolderPathProviderParametersIsInvalid_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IPrePackagingFolderPathProvider prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>().Object;
            IPackagingFolderPathProvider packagingFolderPathProvider = null;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new PackagingService(prePackagingFolderPathProvider, packagingFolderPathProvider, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsInvalid_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IPrePackagingFolderPathProvider prePackagingFolderPathProvider = new Mock<IPrePackagingFolderPathProvider>().Object;
            IPackagingFolderPathProvider packagingFolderPathProvider = new Mock<IPackagingFolderPathProvider>().Object;
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new PackagingService(prePackagingFolderPathProvider, packagingFolderPathProvider, filesystemAccessor);
        }

        #endregion

        #region Package

        [Test]
        public void Package_PrePackagingFolderPathDoesNotexist_ResultIsFalse()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(false);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Package_PackagingFolderPathDoesNotexist_ResultIsFalse()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            string packagingFolderPath = Path.GetFullPath("packaging");

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(false);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Package_FoldersExist_NuSpecFileIsNotFound_ResultIsFalse()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            string packagingFolderPath = Path.GetFullPath("packaging");

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();

            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetFiles(prePackagingFolderPath)).Returns(new List<FileInfo>());

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Package_FoldersExist_NuSpecFileExists_ButItCannotBeOpened_ResultIsFalse()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            string packagingFolderPath = Path.GetFullPath("packaging");

            string nuspecFilePath = Path.GetFullPath("test.nuspec");
            Stream nuspecFileStream = null;

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();

            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetFiles(prePackagingFolderPath)).Returns(new List<FileInfo> { new FileInfo(nuspecFilePath) });
            filesystemAccessorMock.Setup(f => f.GetFileStream(nuspecFilePath)).Returns(nuspecFileStream);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Dasdasdashdjkashdjkhfasdfkj")]
        public void Package_FoldersExist_NuSpecFileExists_ButItsContentIsInvalid_ResultIsFalse(string nuspecFileContent)
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            string packagingFolderPath = Path.GetFullPath("packaging");

            string nuspecFilePath = Path.GetFullPath("test.nuspec");
            Stream nuspecFileStream = TestUtilities.GetStreamReaderForText(nuspecFileContent).BaseStream;

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();

            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetFiles(prePackagingFolderPath)).Returns(new List<FileInfo> { new FileInfo(nuspecFilePath) });
            filesystemAccessorMock.Setup(f => f.GetFileStream(nuspecFilePath)).Returns(nuspecFileStream);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();
            nuspecFileStream.Close();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Package_FoldersExist_NuSpecFileExistsAndIsValid_PackageStreamIsNullBecauseThePackagingFolderDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            Directory.CreateDirectory(prePackagingFolderPath);
            File.WriteAllText(Path.Combine(prePackagingFolderPath, "sample.txt"), "yada yada");

            string packagingFolderPath = Path.GetFullPath("packaging");

            string nuspecFileContent = "<?xml version=\"1.0\"?> <package> <metadata> <id>Packaging.Test</id> <version>1.0.0</version> <authors>NuDeploy.Tests</authors> <owners>NuDeploy.Tests</owners> <requireLicenseAcceptance>false</requireLicenseAcceptance> <description>Packaging Test</description> <releaseNotes></releaseNotes> <copyright>NuDeploy.Tests</copyright> </metadata> </package>";

            string nuspecFilePath = Path.GetFullPath("test.nuspec");
            Stream nuspecFileStream = TestUtilities.GetStreamReaderForText(nuspecFileContent).BaseStream;

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();

            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetFiles(prePackagingFolderPath)).Returns(new List<FileInfo> { new FileInfo(nuspecFilePath) });
            filesystemAccessorMock.Setup(f => f.GetFileStream(nuspecFilePath)).Returns(nuspecFileStream);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();
            nuspecFileStream.Close();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Package_FoldersExist_NuSpecFileExistsAndIsValid_ResultIsTrue()
        {
            // Arrange
            string prePackagingFolderPath = Path.GetFullPath("prepackaging");
            Directory.CreateDirectory(prePackagingFolderPath);
            File.WriteAllText(Path.Combine(prePackagingFolderPath, "sample.txt"), "yada yada");

            string packagingFolderPath = Path.GetFullPath("packaging");
            Directory.CreateDirectory(packagingFolderPath);
            string targetPackagePath = Path.Combine(packagingFolderPath, "package.zip");
            FileStream packageStream = File.Open(targetPackagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);


            string nuspecFilePath = Path.GetFullPath("test.nuspec");
            string nuspecFileContent = "<?xml version=\"1.0\"?> <package> <metadata> <id>Packaging.Test</id> <version>1.0.0</version> <authors>NuDeploy.Tests</authors> <owners>NuDeploy.Tests</owners> <requireLicenseAcceptance>false</requireLicenseAcceptance> <description>Packaging Test</description> <releaseNotes></releaseNotes> <copyright>NuDeploy.Tests</copyright> </metadata> </package>";
            Stream nuspecFileStream = TestUtilities.GetStreamReaderForText(nuspecFileContent).BaseStream;

            var prePackagingFolderPathProviderMock = new Mock<IPrePackagingFolderPathProvider>();
            prePackagingFolderPathProviderMock.Setup(p => p.GetPrePackagingFolderPath()).Returns(prePackagingFolderPath);

            var packagingFolderPathProviderMock = new Mock<IPackagingFolderPathProvider>();
            packagingFolderPathProviderMock.Setup(p => p.GetPackagingFolderPath()).Returns(packagingFolderPath);

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();

            filesystemAccessorMock.Setup(f => f.DirectoryExists(prePackagingFolderPath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.DirectoryExists(packagingFolderPath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetFiles(prePackagingFolderPath)).Returns(new List<FileInfo> { new FileInfo(nuspecFilePath) });
            filesystemAccessorMock.Setup(f => f.GetFileStream(nuspecFilePath)).Returns(nuspecFileStream);

            filesystemAccessorMock.Setup(f => f.GetNewFileStream(It.IsAny<string>())).Returns(packageStream);

            var packagingService = new PackagingService(prePackagingFolderPathProviderMock.Object, packagingFolderPathProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            var result = packagingService.Package();

            nuspecFileStream.Close();
            packageStream.Close();

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(targetPackagePath));
        }

        #endregion
    }
}