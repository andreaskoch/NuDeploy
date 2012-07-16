using System;
using System.IO;
using System.Text;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Transformation
{
    [TestFixture]
    public class ConfigurationFileTransformerTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            IUserInterface userInterface = new Mock<IUserInterface>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var result = new ConfigurationFileTransformer(userInterface, filesystemAccessor);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IUserInterface userInterface = null;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new ConfigurationFileTransformer(userInterface, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IUserInterface userInterface = new Mock<IUserInterface>().Object;
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new ConfigurationFileTransformer(userInterface, filesystemAccessor);
        }

        #endregion

        #region Transform

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Transform_SourceFilePathParameterIsInvalid_ResultIsFalse(string sourceFilePath)
        {
            // Arrange
            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_SourceFileDoesNotExist_ResultIsFalse()
        {
            // Arrange
            var sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(false);

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Transform_TransformationFilePathParameterIsInvalid_ResultIsFalse(string transformationFilePath)
        {
            // Arrange
            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            var sourceFilePath = "source.config";
            var destinationFilePath = "destination.config";

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Transform_DestinationFilePathParameterIsInvalid_ResultIsFalse(string destinationFilePath)
        {
            // Arrange
            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            var sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_TransformationFilePathDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            var userInterfaceMock = new Mock<IUserInterface>();

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(false);

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("")]
        public void Transform_SourceFileIsInvalid_ResultIsFalse(string sourceFileContent)
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.GetTextReader(sourceFilePath)).Returns(() => this.GetStreamReaderForText(sourceFileContent));

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_SourceFileThrowsIOExceptionOnFirstAccess_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.GetTextReader(sourceFilePath)).Throws(new IOException());

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("")]
        [TestCase("not xml at all")]
        [TestCase(null)]
        public void Transform_SourceFileIsValid_TransformationFileContentIsInvalid_ResultIsFalse(string transformationFileContent)
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            string sourceFileContent = "<configuration><appSettings><add key=\"Key1\" value=\"Not-Transformed\"/></appSettings></configuration>";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetTextReader(sourceFilePath)).Returns(() => this.GetStreamReaderForText(sourceFileContent));
            filesystemAccessorMock.Setup(f => f.GetFileContent(transformationFilePath)).Returns(transformationFileContent);

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_SourceFileIsValid_TransformationFileThrowsIOExceptionOnFirstAccess_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            string sourceFileContent = "<configuration><appSettings><add key=\"Key1\" value=\"Not-Transformed\"/></appSettings></configuration>";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetTextReader(sourceFilePath)).Returns(() => this.GetStreamReaderForText(sourceFileContent));
            filesystemAccessorMock.Setup(f => f.GetFileContent(transformationFilePath)).Throws(new IOException());

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_SourceFileIsValid_TransformationFileIsValid_DestinationDocumentIsWrittenUsingTextWriter_TextWriterContainsTransformedXml()
        {
            // Arrange
            string sourceFilePath = "source.config";
            var transformationFilePath = "transform.config";
            var destinationFilePath = "destination.config";

            string sourceFileContent = "<configuration><appSettings><add key=\"Key1\" value=\"Not-Transformed\"/></appSettings></configuration>";

            string uniqueString = Guid.NewGuid().ToString();
            string transformationFileContent =
                "<configuration xmlns:xdt=\"http://schemas.microsoft.com/XML-Document-Transform\"><appSettings xdt:Transform=\"Replace\"><add key=\"Key1\" value=\""
                + uniqueString + "\"/></appSettings></configuration>";

            var userInterfaceMock = new Mock<IUserInterface>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            filesystemAccessorMock.Setup(f => f.FileExists(sourceFilePath)).Returns(true);
            filesystemAccessorMock.Setup(f => f.FileExists(transformationFilePath)).Returns(true);

            filesystemAccessorMock.Setup(f => f.GetTextReader(sourceFilePath)).Returns(() => this.GetStreamReaderForText(sourceFileContent));
            filesystemAccessorMock.Setup(f => f.GetFileContent(transformationFilePath)).Returns(transformationFileContent);

            var destinationFileContent = new StringBuilder();
            var stringWriter = new StringWriter(destinationFileContent);
            filesystemAccessorMock.Setup(f => f.GetTextWriter(destinationFilePath)).Returns(stringWriter);

            var configurationFileTransformer = new ConfigurationFileTransformer(userInterfaceMock.Object, filesystemAccessorMock.Object);

            // Act
            bool result = configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(destinationFileContent.ToString());
            Assert.IsTrue(destinationFileContent.ToString().Contains(uniqueString));
        }

        #endregion

        #region Utility Methods

        private StreamReader GetStreamReaderForText(string text)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }

        #endregion
    }
}