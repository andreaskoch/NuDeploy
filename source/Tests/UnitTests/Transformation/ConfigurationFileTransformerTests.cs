using System;

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

        #endregion
    }
}