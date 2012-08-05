using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Installation;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
{
    [TestFixture]
    public class NugetPackageExtractorTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Assert
            Assert.IsNotNull(nugetPackageExtractor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NugetPackageExtractorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Act
            new NugetPackageExtractor(null);
        }

        #endregion

        #region Extract

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extract_PackageParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IPackage package = null;
            string targetFolder = Environment.CurrentDirectory;

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);            

            // Act
            nugetPackageExtractor.Extract(package, targetFolder);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Extract_TargetFolderParameterIsNull_ArgumentExceptionIsThrown(string targetFolder)
        {
            // Arrange
            var package = new Mock<IPackage>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            nugetPackageExtractor.Extract(package.Object, targetFolder);
        }

        [Test]
        public void Extract_PackageFolderAlreadyExists_DeleteDirectoryIsCalled()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;
            var files = new List<IPackageFile>();

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);
            var packageDirectory = Path.Combine(targetFolder, string.Format("{0}.{1}", packageId, version));

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(packageDirectory)).Returns(true);

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            filesystemAccessor.Verify(f => f.DeleteDirectory(packageDirectory), Times.Once());
        }

        [Test]
        public void Extract_PackageFolderAlreadyExists_DeleteDirectoryFails_ResultIsNull()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;
            var files = new List<IPackageFile>();

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);
            var packageDirectory = Path.Combine(targetFolder, string.Format("{0}.{1}", packageId, version));

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DirectoryExists(packageDirectory)).Returns(true);
            filesystemAccessor.Setup(f => f.DeleteDirectory(packageDirectory)).Returns(false);

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Extract_PackageContainsNoFiles_ResultIsValidPackageInfoObject()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            var files = new List<IPackageFile>();

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);
            var packageDirectory = Path.Combine(targetFolder, string.Format("{0}.{1}", packageId, version));

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();


            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(packageId, result.Id);
            Assert.AreEqual(version, result.Version);
            Assert.AreEqual(packageDirectory, result.Folder);
            Assert.AreEqual(false, result.IsInstalled);
        }

        [Test]
        public void Extract_PackageContainsFiles_PackageFileReadStreamIsNull_ResultIsNull()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            string file1Path = "tools\\file1.txt";
            Stream file1Stream = null;

            var file1 = new Mock<IPackageFile>();

            file1.Setup(f => f.Path).Returns(file1Path);
            file1.Setup(f => f.GetStream()).Returns(file1Stream);

            var files = new List<IPackageFile> { file1.Object };

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Extract_PackageContainsFiles_PackageFileReadStreamThrowsException_ResultIsNull()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            string file1Path = "tools\\file1.txt";
            var file1 = new Mock<IPackageFile>();
            file1.Setup(f => f.Path).Returns(file1Path);
            file1.Setup(f => f.GetStream()).Throws(new Exception());

            var files = new List<IPackageFile> { file1.Object };

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Extract_PackageContainsFiles_WriteStreamIsNull_ResultIsNull()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            string file1Path = "tools\\file1.txt";
            string file1Content = Guid.NewGuid().ToString();
            Stream file1Stream = TestUtilities.GetStreamReaderForText(file1Content).BaseStream;

            var file1 = new Mock<IPackageFile>();
            file1.Setup(f => f.Path).Returns(file1Path);
            file1.Setup(f => f.GetStream()).Returns(file1Stream);

            var files = new List<IPackageFile> { file1.Object };

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            Stream writeStream = null;
            filesystemAccessor.Setup(f => f.GetWriteStream(It.Is<string>(s => s.EndsWith(file1Path)))).Returns(writeStream);

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Extract_PackageContainsFiles_WriteStreamThrowsException_ResultIsNull()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            string file1Path = "tools\\file1.txt";
            string file1Content = Guid.NewGuid().ToString();
            Stream file1Stream = TestUtilities.GetStreamReaderForText(file1Content).BaseStream;

            var file1 = new Mock<IPackageFile>();
            file1.Setup(f => f.Path).Returns(file1Path);
            file1.Setup(f => f.GetStream()).Returns(file1Stream);

            var files = new List<IPackageFile> { file1.Object };

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetWriteStream(It.Is<string>(s => s.EndsWith(file1Path)))).Throws(new Exception());

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Extract_PackageContainsFiles_ContentOfEachFileIsWrittenToDisc()
        {
            // Arrange
            string targetFolder = Environment.CurrentDirectory;

            string file1Path = "tools\\file1.txt";
            string file1Content = Guid.NewGuid().ToString();
            Stream file1Stream = TestUtilities.GetStreamReaderForText(file1Content).BaseStream;
            var file1 = new Mock<IPackageFile>();
            file1.Setup(f => f.Path).Returns(file1Path);
            file1.Setup(f => f.GetStream()).Returns(file1Stream);

            var files = new List<IPackageFile> { file1.Object };

            string packageId = "Package.A";
            var version = new SemanticVersion(1, 0, 0, 0);
            var packageDirectory = Path.Combine(targetFolder, string.Format("{0}.{1}", packageId, version));

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(version);
            package.Setup(p => p.GetFiles()).Returns(files);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            filesystemAccessor.Setup(f => f.DirectoryExists(packageDirectory)).Returns(true);
            filesystemAccessor.Setup(f => f.DeleteDirectory(packageDirectory)).Returns(true);

            var writeStream = new MemoryStream();
            var streamWriter = new StreamWriter(writeStream);
            filesystemAccessor.Setup(f => f.GetWriteStream(It.Is<string>(s => s.EndsWith(file1Path)))).Returns(streamWriter.BaseStream);

            var nugetPackageExtractor = new NugetPackageExtractor(filesystemAccessor.Object);

            // Act
            var result = nugetPackageExtractor.Extract(package.Object, targetFolder);

            // Assert
            var bytes = writeStream.ReadAllBytes();
            string newContentOfOldFile = Encoding.UTF8.GetString(bytes);

            Assert.IsNotNull(result);
            Assert.AreEqual(file1Content, newContentOfOldFile);

            file1.Verify(f => f.GetStream(), Times.Once());
            filesystemAccessor.Verify(f => f.GetWriteStream(It.Is<string>(s => s.StartsWith(packageDirectory))), Times.Once());
        }

        #endregion
    }
}