using System;
using System.IO;

using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.FileSystem
{
    [TestFixture]
    public class PhysicalFilesystemAccessorTests
    {
        private const string SampleFileFolder = "samples";

        private IFilesystemAccessor filesystemAccessor;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.filesystemAccessor = new PhysicalFilesystemAccessor();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            if (Directory.Exists(SampleFileFolder))
            {
                Directory.Delete(SampleFileFolder, true);
            }

            Directory.CreateDirectory(SampleFileFolder);
        }

        #region FileExists

        [Test]
        public void FileExists_SuppliedPathIsNull_ResultIsFalse()
        {
            // Arrange
            string path = null;

            // Act
            bool result = this.filesystemAccessor.FileExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void FileExists_SuppliedPathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string path = string.Empty;

            // Act
            bool result = this.filesystemAccessor.FileExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void FileExists_SuppliedPathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string path = " ";

            // Act
            bool result = this.filesystemAccessor.FileExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void FileExists_SuppliedPathIsDirectory_ResultIsFalse()
        {
            // Arrange
            string path = Environment.CurrentDirectory;
            
            // Act
            bool result = this.filesystemAccessor.FileExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void FileExists_SuppliedPathIsExistingFile_ResultIsTrue()
        {
            // Arrange
            var path = this.CreateFile("test-file.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.FileExists(path);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region DirectoryExists

        [Test]
        public void DirectoryExists_SuppliedPathIsNull_ResultIsFalse()
        {
            // Arrange
            string path = null;

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_SuppliedPathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string path = string.Empty;

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_SuppliedPathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string path = " ";

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region utility methods

        private FileInfo CreateFile(string relativeFilePath)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, SampleFileFolder, relativeFilePath);
            File.WriteAllText(filePath, Guid.NewGuid().ToString());
            var fileInfo = new FileInfo(filePath);
            return fileInfo;
        }

        #endregion
    }
}
