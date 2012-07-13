using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Logging;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.FileSystem
{
    [TestFixture]
    public class PhysicalFilesystemAccessorTests
    {
        private const string SampleFileFolder = "samples";

        private IFilesystemAccessor filesystemAccessor;

        private IEncodingProvider encodingProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var logger = new Mock<IActionLogger>();
            this.encodingProvider = new DefaultFileEncodingProvider();
            this.filesystemAccessor = new PhysicalFilesystemAccessor(logger.Object, this.encodingProvider);
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
        public void FileExists_SuppliedPathIsNotExistingFile_ResultIsFalse()
        {
            // Arrange
            var path = "There-Is-No-Way-This-File-Can-Exist-salkfjaskjksad43242jf.txt";

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

        [Test]
        public void DirectoryExists_SuppliedPathIsAnExistingFile_ResultIsFalse()
        {
            // Arrange
            var path = this.CreateFile("test-file.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_SuppliedPathIsAnNonExistingFile_ResultIsFalse()
        {
            // Arrange
            var path = "There-Is-No-Way-This-File-Can-Exist-salkfjaskjksad43242jf.txt";

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_SuppliedPathIsNonExistingDirectory_ResultIsFalse()
        {
            // Arrange
            var path = "NonExistingTestDirectory";

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_SuppliedPathIsExistingDirectory_ResultIsTrue()
        {
            // Arrange
            var path = this.CreateDirectory("TestDirectory").FullName;

            // Act
            bool result = this.filesystemAccessor.DirectoryExists(path);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region MoveFile

        [Test]
        public void MoveFile_SourceFilePathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = string.Empty;

            string targetFilePath = string.Empty;

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFilePathIsNull_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = null;

            string targetFilePath = this.GetPath("Target-File.txt");

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFilePathIsWhiteSpace_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = " ";

            string targetFilePath = this.GetPath("Target-File.txt");

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFilePathDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = "Non-Existing-File-3123123123123.txt";

            string targetFilePath = this.GetPath("Target-File.txt");

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileParameterIsNull_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;

            string targetFilePath = null;

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileParameterIsEmpty_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;

            string targetFilePath = string.Empty;

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileParameterIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;

            string targetFilePath = " ";

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileExists_ResultIsTrue_TargetFileContainsContentOfSourceFile()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;
            string sourceContent = File.ReadAllText(sourceFilePath);

            string targetFilePath = this.CreateFile("Target-File.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            string targetContent = File.ReadAllText(targetFilePath);
            Assert.IsTrue(result);
            Assert.AreEqual(sourceContent, targetContent);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileIsInUse_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;
            string targetFilePath = this.CreateFile("Target-File.txt").FullName;

            var targetFileStreamReader = new StreamReader(targetFilePath);

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
            targetFileStreamReader.Close();
        }

        [Test]
        public void MoveFile_SourceFileExist_SourceFileIsInUse_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;
            string targetFilePath = this.CreateFile("Target-File.txt").FullName;

            var sourceFileStreamReader = new StreamReader(sourceFilePath);

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            Assert.IsFalse(result);
            sourceFileStreamReader.Close();
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileDoesNotExist_ResultIsTrue_SourceFileIsRemoved_TargetFileContainsContentOfSource()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;
            string sourceFileContent = File.ReadAllText(sourceFilePath);

            string targetFilePath = this.CreateFile("Target-File.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            string targetFileContent = File.ReadAllText(targetFilePath);

            Assert.IsTrue(result);
            Assert.IsFalse(File.Exists(sourceFilePath));
            Assert.IsTrue(File.Exists(targetFilePath));
            Assert.AreEqual(sourceFileContent, targetFileContent);
        }

        [Test]
        public void MoveFile_SourceFileExist_TargetFileExist_ResultIsTrue_TargetFileContentChanged()
        {
            // Arrange
            string sourceFilePath = this.CreateFile("File-Move-Test-Source.txt").FullName;
            string sourceFileContent = File.ReadAllText(sourceFilePath);

            string targetFilePath = this.CreateFile("Target-File.txt").FullName;
            string targetFileContentBefore = File.ReadAllText(targetFilePath);

            // Act
            bool result = this.filesystemAccessor.MoveFile(sourceFilePath, targetFilePath);

            // Assert
            string targetFileContentAfter = File.ReadAllText(targetFilePath);

            Assert.IsTrue(result);
            Assert.AreNotEqual(targetFileContentBefore, targetFileContentAfter);
            Assert.AreEqual(sourceFileContent, targetFileContentAfter);
        }

        #endregion

        #region DeleteFile

        [Test]
        public void DeleteFile_FilePathIsNull_ResultIsFalse()
        {
            // Arrange
            string filePath = null;

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteFile_FilePathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string filePath = string.Empty;

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteFile_FilePathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string filePath = " ";

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteFile_FileDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string filePath = "There-Is-No-Way-This-File-Can-Exist-salkfjaskjksad43242jf.txt";

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteFile_FileExistButIsInUse_ResultIsFalse()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-Is-Going-To-Be-Deleted.txt").FullName;
            var streamReader = new StreamReader(filePath);

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsFalse(result);
            streamReader.Close();
        }

        [Test]
        public void DeleteFile_FileExist_ResultIsTrue_FileDoesNoLongerExist()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-Is-Going-To-Be-Deleted.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.DeleteFile(filePath);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(File.Exists(filePath));
        }

        #endregion

        #region DeleteDirectory

        [Test]
        public void DeleteDirectory_FolderPathIsNull_ResultIsFalse()
        {
            // Arrange
            string path = null;

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsFalse(result);            
        }

        [Test]
        public void DeleteDirectory_FolderPathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string path = string.Empty;

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_FolderPathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string path = " ";

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_FolderDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string path = this.GetPath("non-existing-folder");

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_FolderExistsButOneFileInItIsInUse_ResultIsFalse()
        {
            // Arrange
            string path = this.CreateDirectory("new folder").FullName;
            string filePath = this.CreateFile("new folder\\test-file.txt").FullName;
            var streamReader = new StreamReader(filePath);

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsFalse(result);
            streamReader.Close();
        }

        [Test]
        public void DeleteDirectory_FolderExists_FolderIsEmpty_ResultIsTrue_FolderIsRemoved()
        {
            // Arrange
            string path = this.CreateDirectory("new folder").FullName;

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(Directory.Exists(path));
        }

        [Test]
        public void DeleteDirectory_FolderExists_ContainsContent_ResultIsTrue_FolderIsRemoved()
        {
            // Arrange
            string path = this.CreateDirectory("new folder").FullName;
            this.CreateFile("new folder\\test-file.txt");

            // Act
            bool result = this.filesystemAccessor.DeleteDirectory(path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(Directory.Exists(path));
        }

        #endregion

        #region GetFileContent

        [Test]
        public void GetFileContent_FilePathIsNull_ResultIsNull()
        {
            // Arrange
            string filePath = null;

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetFileContent_FilePathIsEmpty_ResultIsNull()
        {
            // Arrange
            string filePath = string.Empty;

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetFileContent_FilePathIsWhitespace_ResultIsNull()
        {
            // Arrange
            string filePath = " ";

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetFileContent_FileDoesNotExist_ResultIsNull()
        {
            // Arrange
            string filePath = "There-Is-No-Way-This-File-Can-Exist-salkfjaskjksad43242jf.txt";

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetFileContent_FileExists_ButIsBeingWrittenTo_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("File-That-Is-Being-Written-To-By-Another-Process.txt").FullName;
            var streamWriter = new StreamWriter(filePath);

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNull(result);
            streamWriter.Close();
        }

        [Test]
        public void GetFileContent_FileExists_IsBeingRead_ResultIsNotNull()
        {
            // Arrange
            string filePath = this.CreateFile("File-That-Is-Being-Read-By-Another-Process.txt").FullName;
            var streamWriter = new StreamReader(filePath);

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.IsNotNull(result);
            streamWriter.Close();
        }

        [Test]
        public void GetFileContent_FileExists_ResultIsContentOfTheSpecifiedFilePath()
        {
            // Arrange
            string filePath = this.CreateFile("File-That-Is-Being-Read-By-Another-Process.txt").FullName;
            var fileContent = this.GetFileContent(filePath);

            // Act
            string result = this.filesystemAccessor.GetFileContent(filePath);

            // Assert
            Assert.AreEqual(fileContent, result);
        }

        #endregion

        #region WriteContentToFile

        [Test]
        public void WriteContentToFile_ContentIsNull_ResultIsFalse_TargetFileIsNotCreated()
        {
            // Arrange
            string content = null;
            string filePath = this.GetPath("Target-File.txt");

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void WriteContentToFile_FilePathIsNull_ResultIsFalse()
        {
            // Arrange
            string content = "Some Content";
            string filePath = null;

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void WriteContentToFile_FilePathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string content = "Some Content";
            string filePath = string.Empty;

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void WriteContentToFile_FilePathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string content = "Some Content";
            string filePath = " ";

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void WriteContentToFile_TargetFileExistAndIsBeingRead_ResultIsFalse()
        {
            // Arrange
            string content = "Some Content";
            string filePath = this.CreateFile("Target-File.txt").FullName;
            var streamReader = new StreamReader(filePath);

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
            streamReader.Close();
        }

        [Test]
        public void WriteContentToFile_TargetFileExistAndIsWrittenTo_ResultIsFalse()
        {
            // Arrange
            string content = "Some Content";
            string filePath = this.CreateFile("Target-File.txt").FullName;
            var streamWriter = new StreamWriter(filePath);

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsFalse(result);
            streamWriter.Close();
        }

        [Test]
        public void WriteContentToFile_TargetFileExist_ResultIsTrue_FileContainsSuppliedContent()
        {
            // Arrange
            string content = "Some Content";
            string filePath = this.CreateFile("Target-File.txt").FullName;
            string previousFileContent = this.GetFileContent(filePath);

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(content, this.GetFileContent(filePath));
            Assert.AreNotEqual(previousFileContent, this.GetFileContent(filePath));
        }

        [Test]
        public void WriteContentToFile_ContentIsNotNull_FilePathIsValidAndDoesNotExist_ResultIsTrue_FileContainsSuppliedContent()
        {
            // Arrange
            string content = "Some Content";
            string filePath = this.GetPath("Target-File.txt");

            // Act
            bool result = this.filesystemAccessor.WriteContentToFile(content, filePath);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(content, this.GetFileContent(filePath));
        }

        #endregion

        #region GetTextReader

        [Test]
        public void GetTextReader_FilePathIsNull_ResultIsNull()
        {
            // Arrange
            string filePath = null;

            // Act
            var textReader = this.filesystemAccessor.GetTextReader(filePath);

            // Assert
            Assert.IsNull(textReader);
        }

        [Test]
        public void GetTextReader_FilePathIsEmpty_ResultIsNull()
        {
            // Arrange
            string filePath = string.Empty;

            // Act
            var textReader = this.filesystemAccessor.GetTextReader(filePath);

            // Assert
            Assert.IsNull(textReader);
        }

        [Test]
        public void GetTextReader_FilePathIsWhitespace_ResultIsNull()
        {
            // Arrange
            string filePath = " ";

            // Act
            var textReader = this.filesystemAccessor.GetTextReader(filePath);

            // Assert
            Assert.IsNull(textReader);
        }

        [Test]
        public void GetTextReader_FileDoesNotExist_ResultIsNull()
        {
            // Arrange
            string filePath = "There-Is-No-Way-This-File-Can-Exist-salkfjaskjksad43242jf.txt";

            // Act
            var textReader = this.filesystemAccessor.GetTextReader(filePath);

            // Assert
            Assert.IsNull(textReader);
        }

        [Test]
        public void GetTextReader_FileExists_ButIsBeingWrittenTo_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-Is-Being-Written-To.txt").FullName;
            var streamWriter = new StreamWriter(filePath);

            // Act
            var textReader = this.filesystemAccessor.GetTextReader(filePath);

            // Assert
            Assert.IsNull(textReader);
            streamWriter.Close();
        }

        [Test]
        public void GetTextReader_FileExists_ButIsBeingReadByAnotherProcess_ResultIsNotNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-Is-Being-Read.txt").FullName;
            var streamReader = new StreamReader(filePath);

            // Act
            using (var textReader = this.filesystemAccessor.GetTextReader(filePath))
            {
                // Assert
                Assert.IsNotNull(textReader);
                streamReader.Close();                
            }
        }

        [Test]
        public void GetTextReader_FileExists_ResultIsNotNull_TextReaderReturnsFileContent()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;
            string fileContent = this.GetFileContent(filePath);

            // Act
            using (var textReader = this.filesystemAccessor.GetTextReader(filePath))
            {
                // Assert
                Assert.IsNotNull(textReader);
                Assert.AreEqual(fileContent, textReader.ReadToEnd());                
            }
        }

        #endregion

        #region GetTextWriter

        [Test]
        public void GetTextWriter_FilePathIsNull_ResultIsNull()
        {
            // Arrange
            string filePath = null;

            // Act
            var textWriter = this.filesystemAccessor.GetTextWriter(filePath);

            // Assert
            Assert.IsNull(textWriter);
        }

        [Test]
        public void GetTextWriter_FilePathIsEmpty_ResultIsNull()
        {
            // Arrange
            string filePath = string.Empty;

            // Act
            var textWriter = this.filesystemAccessor.GetTextWriter(filePath);

            // Assert
            Assert.IsNull(textWriter);
        }

        [Test]
        public void GetTextWriter_FilePathIsWhitespace_ResultIsNull()
        {
            // Arrange
            string filePath = " ";

            // Act
            var textWriter = this.filesystemAccessor.GetTextWriter(filePath);

            // Assert
            Assert.IsNull(textWriter);
        }

        [Test]
        public void GetTextWriter_FileExists_AndIsBeingReadByAnotherProcess_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-is-being-read.txt").FullName;
            var streamReader = new StreamReader(filePath);

            // Act
            var textWriter = this.filesystemAccessor.GetTextWriter(filePath);

            // Assert
            Assert.IsNull(textWriter);
            streamReader.Close();
        }

        [Test]
        public void GetTextWriter_FileExists_AndIsWrittenToByAnotherProcess_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File-That-Is-Being-Written-To.txt").FullName;
            var streamWriter = new StreamWriter(filePath);

            // Act
            var textWriter = this.filesystemAccessor.GetTextWriter(filePath);

            // Assert
            Assert.IsNull(textWriter);
            streamWriter.Close();
        }

        [Test]
        public void GetTextWriter_FileExists_ResultIsNotNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;

            // Act
            using (var textWriter = this.filesystemAccessor.GetTextWriter(filePath))
            {
                // Assert
                Assert.IsNotNull(textWriter);                
            }
        }

        [Test]
        public void GetTextWriter_FileDoesNotExist_ResultIsNotNull()
        {
            // Arrange
            string filePath = this.GetPath("new-file.txt");

            // Act
            using (var textWriter = this.filesystemAccessor.GetTextWriter(filePath))
            {
                // Assert
                Assert.IsNotNull(textWriter);
            }
        }

        [Test]
        public void GetTextWriter_FileDoesNotExist_FileIsCreated_TextIsWrittenToFile()
        {
            // Arrange
            string filePath = this.GetPath("new-file.txt");
            string fileContent = Guid.NewGuid().ToString();

            // Act
            using (var textWriter = this.filesystemAccessor.GetTextWriter(filePath))
            {
                textWriter.Write(fileContent);
            }

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(fileContent, this.GetFileContent(filePath));
        }

        #endregion

        #region GetNewFileStream

        [Test]
        public void GetNewFileStream_FilePathIsNull_ResultIsNull()
        {
            // Arrange
            string filePath = null;

            // Act
            var stream = this.filesystemAccessor.GetNewFileStream(filePath);

            // Assert
            Assert.IsNull(stream);
        }

        [Test]
        public void GetNewFileStream_FilePathIsEmpty_ResultIsNull()
        {
            // Arrange
            string filePath = string.Empty;

            // Act
            var stream = this.filesystemAccessor.GetNewFileStream(filePath);

            // Assert
            Assert.IsNull(stream);
        }

        [Test]
        public void GetNewFileStream_FilePathIsWhitespace_ResultIsNull()
        {
            // Arrange
            string filePath = " ";

            // Act
            var stream = this.filesystemAccessor.GetNewFileStream(filePath);

            // Assert
            Assert.IsNull(stream);
        }

        [Test]
        public void GetNewFileStream_FileExists_ButIsBeingRead_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;
            var reader = new StreamReader(filePath);

            // Act
            var stream = this.filesystemAccessor.GetNewFileStream(filePath);

            // Assert
            Assert.IsNull(stream);
            reader.Close();
        }

        [Test]
        public void GetNewFileStream_FileExists_ButIsBeingWrittenTo_ResultIsNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;
            var writer = new StreamWriter(filePath);

            // Act
            var stream = this.filesystemAccessor.GetNewFileStream(filePath);

            // Assert
            Assert.IsNull(stream);
            writer.Close();
        }

        [Test]
        public void GetNewFileStream_FileDoesNotExist_ResultIsNotNull_FileIsCreated()
        {
            // Arrange
            string filePath = this.GetPath("Non-Existing-File.txt");

            // Act
            using (var stream = this.filesystemAccessor.GetNewFileStream(filePath))
            {
                // Assert
                Assert.IsNotNull(stream);
                Assert.IsTrue(File.Exists(filePath));
            }
        }

        [Test]
        public void GetNewFileStream_FileExists_ResultIsNotNull()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;

            // Act
            using (var stream = this.filesystemAccessor.GetNewFileStream(filePath))
            {
                // Assert
                Assert.IsNotNull(stream);                
            }
        }

        [Test]
        public void GetNewFileStream_FileExists_StreamReaderReturnsEmptyStringBecauseTheFileIsOverridden()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;
            string fileContent = this.GetFileContent(filePath);

            // Act
            using (var stream = this.filesystemAccessor.GetNewFileStream(filePath))
            {
                TextReader textReader = new StreamReader(stream);
                string contentReadFromStream = textReader.ReadToEnd();

                // Assert
                Assert.IsNotNull(stream);
                Assert.AreNotEqual(fileContent, contentReadFromStream);
                Assert.IsNullOrEmpty(contentReadFromStream);
            }
        }

        [Test]
        public void GetNewFileStream_FileExists_StreamCanBeWrittenTo_OriginalFileContentIsCompletelyOverridden()
        {
            // Arrange
            string filePath = this.CreateFile("Existing-File.txt").FullName;
            string oldFileContent = this.GetFileContent(filePath);

            // Act
            using (var stream = this.filesystemAccessor.GetNewFileStream(filePath))
            {
                TextWriter textWriter = new StreamWriter(stream);
                textWriter.Write("New Content");
                textWriter.Flush();
            }

            // Assert
            string newFileContent = this.GetFileContent(filePath);
            Assert.AreNotEqual(oldFileContent, newFileContent);
            Assert.IsFalse(newFileContent.Contains(oldFileContent));
        }

        #endregion

        #region CreateDirectory

        [Test]
        public void CreateDirectory_PathIsNull_ResultIsFalse()
        {
            // Arrange
            string path = null;

            // Act
            bool result = this.filesystemAccessor.CreateDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateDirectory_PathIsEmpty_ResultIsFalse()
        {
            // Arrange
            string path = string.Empty;

            // Act
            bool result = this.filesystemAccessor.CreateDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateDirectory_PathIsWhitespace_ResultIsFalse()
        {
            // Arrange
            string path = " ";

            // Act
            bool result = this.filesystemAccessor.CreateDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateDirectory_PathIsExistingDirectory_ResultIsFalse()
        {
            // Arrange
            string path = this.CreateDirectory("existing-directory").FullName;

            // Act
            bool result = this.filesystemAccessor.CreateDirectory(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateDirectory_PathIsValid_DoesNotExistYet_ResultIsTrue()
        {
            // Arrange
            string path = this.GetPath("new-directory");

            // Act
            bool result = this.filesystemAccessor.CreateDirectory(path);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region CopyFile

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CopyFile_InvalidSourceFile_ResultIsFalse(string sourceFile)
        {
            // Arrange
            string targetFile = this.GetPath("target-file.txt");

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CopyFile_SourceFileDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string sourceFile = this.GetPath("Non-existing-file.txt");
            string targetFile = this.GetPath("target-file.txt");

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CopyFile_InvalidTargetFile_ResultIsFalse(string targetFile)
        {
            // Arrange
            string sourceFile = this.CreateFile("source-file.txt").FullName;

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFilePathDoesNotExist_PathOfTargetFileIsCreated()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;

            string targetPath = this.GetPath("some\\very\\nested\\folder");
            string targetFileName = "file-in-non-existing-folder.txt";
            string targetFile = string.Format("{0}\\{1}", targetPath, targetFileName);

            // Act
            this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsTrue(Directory.Exists(targetPath));
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFilePathDoesNotExist_FileIsCopied()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.GetPath("some\\very\\nested\\folder\\file-in-non-existing-folder.txt");

            // Act
            this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsTrue(File.Exists(targetFile));
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFileExist_TargetFileIsOverridden()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.CreateFile("existing-target-file.txt").FullName;

            string sourceFileContent = this.GetFileContent(sourceFile);
            string previousTargetFileContent = this.GetFileContent(targetFile);


            // Act
            this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            string newTargetFileContent = this.GetFileContent(targetFile);
            Assert.IsTrue(File.Exists(targetFile));
            Assert.AreNotEqual(previousTargetFileContent, newTargetFileContent);
            Assert.AreEqual(sourceFileContent, newTargetFileContent);
        }

        [Test]
        public void CopyFile_SourceFileExist_SourceIsBeingRead_ResultIsTrue()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.GetPath("target-file.txt");
            var reader = new StreamReader(sourceFile);

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsTrue(result);
            reader.Close();
        }

        [Test]
        public void CopyFile_SourceFileExist_SourceIsBeingWrittenTo_ResultIsTrue()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.GetPath("target-file.txt");
            var writer = new StreamWriter(sourceFile);
            writer.Write("some text");

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsTrue(result);
            writer.Close();
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFileExist_ButIsBeingRead_ResultIsFalse()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.CreateFile("existing-target-file.txt").FullName;
            var reader = new StreamReader(targetFile);

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsFalse(result);
            reader.Close();
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFileExist_ButIsWrittenTo_ResultIsFalse()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.CreateFile("existing-target-file.txt").FullName;
            var writer = new StreamWriter(targetFile);

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsFalse(result);
            writer.Close();
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFileIsValid_ResultIsTrue()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string targetFile = this.GetPath("target-file.txt");

            // Act
            bool result = this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CopyFile_SourceFileExist_TargetFileIsValid_ContentIsCopied()
        {
            // Arrange
            string sourceFile = this.CreateFile("existing-file.txt").FullName;
            string sourceContent = this.GetFileContent(sourceFile);

            string targetFile = this.GetPath("target-file.txt");

            // Act
            this.filesystemAccessor.CopyFile(sourceFile, targetFile);

            // Assert
            string targetFileContent = this.GetFileContent(targetFile);
            Assert.AreEqual(sourceContent, targetFileContent);
        }

        #endregion

        #region EnsureParentDirectoryExists

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void EnsurePathExists_PathIsInvalid_ResultIsFalse(string path)
        {
            // Act
            bool result = this.filesystemAccessor.EnsureParentDirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void EnsurePathExists_PathIsValid_ButDoesNotYetExist_ResultIsTrue()
        {
            // Arrange
            string path = this.GetPath("some","very", "nested", "path", "file.txt");

            // Act
            bool result = this.filesystemAccessor.EnsureParentDirectoryExists(path);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void EnsurePathExists_PathIsValid_ButDoesNotYetExist_DirectoryIsCreated()
        {
            // Arrange
            string directory = this.GetPath("some", "very", "nested", "path");
            string path = Path.Combine(directory, "file.txt");

            // Act
            this.filesystemAccessor.EnsureParentDirectoryExists(path);

            // Assert
            Assert.IsTrue(Directory.Exists(directory));
        }

        [Test]
        public void EnsurePathExists_PathIsValid_ButDoesNotYetExist_ButCannotBeCreatedBecauseItIsToLong_ResultIsFalse()
        {
            // Arrange
            string directory = this.GetPath(new string('s', 260));
            string path = Path.Combine(directory, "file.txt");

            // Act
            bool result = this.filesystemAccessor.EnsureParentDirectoryExists(path);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region utility methods

        private string GetFileContent(string filePath)
        {
            return File.ReadAllText(filePath, this.encodingProvider.GetEncoding());
        }

        private string GetPath(params string[] relativeFilePath)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, SampleFileFolder, Path.Combine(relativeFilePath));
            return filePath;
        }

        private FileInfo CreateFile(string relativeFilePath)
        {
            string filePath = this.GetPath(relativeFilePath);
            File.WriteAllText(filePath, Guid.NewGuid().ToString());
            var fileInfo = new FileInfo(filePath);
            return fileInfo;
        }

        private DirectoryInfo CreateDirectory(string relativeDirectoryPath)
        {
            string directoryPath = this.GetPath(relativeDirectoryPath);
            Directory.CreateDirectory(directoryPath);

            var directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo;
        }

        #endregion
    }
}
