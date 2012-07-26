using System;
using System.Collections.Generic;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Transformation
{
    [TestFixture]
    public class ConfigurationFileTransformationServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // Act
            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Assert
            Assert.IsNotNull(configurationFileTransformationService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // Act
            new ConfigurationFileTransformationService(null, configurationFileTransformer.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConfigurationFileTransformerParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new ConfigurationFileTransformationService(filesystemAccessor.Object, null);
        }

        #endregion

        #region TransformConfigurationFiles

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void TransformConfigurationFiles_BaseDirectoryPathParameterIsInvalid_ArgumentExceptionIsThrown(string baseDirectoryPath)
        {
            // Arrange
            var transformationProfileNames = new string[] { };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformConfigurationFiles_TransformationProfileNamesIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            string[] transformationProfileNames = null;

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);
        }

        [Test]
        public void TransformConfigurationFiles_TransformationProfileNamesIsEmpty_ResultIsTrue()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            string[] transformationProfileNames = new string[] { };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            bool result = configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TransformConfigurationFiles_NoConfigurationFilesFound_ResultIsTrue()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            var transformationProfileNames = new[] { "PROD" };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // prepare filesystem accessor
            var configurationFiles = new List<FileInfo>();
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(folder => folder.StartsWith(baseDirectoryPath)))).Returns(configurationFiles);

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            bool result = configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TransformConfigurationFiles_NoConfigurationFilesFound_TransformIsNotExecuted()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            var transformationProfileNames = new[] { "PROD" };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // prepare filesystem accessor
            var configurationFiles = new List<FileInfo>();
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(folder => folder.StartsWith(baseDirectoryPath)))).Returns(configurationFiles);

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert
            configurationFileTransformer.Verify(t => t.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void TransformConfigurationFiles_ConfigurationFilesFound_TransformIsExecutedForEachFileAndProfile()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            var transformationProfileNames = new[] { "PROD" };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // prepare filesystem accessor
            var configurationFiles = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(baseDirectoryPath, "websites", "website.A", "web.config")),
                    new FileInfo(Path.Combine(baseDirectoryPath, "webapplications", "webapp", "web.config")),
                    new FileInfo(Path.Combine(baseDirectoryPath, "applications", "some-app", "app.config"))
                };
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(folder => folder.StartsWith(baseDirectoryPath)))).Returns(configurationFiles);

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert
            foreach (var configurationFile in configurationFiles)
            {
                foreach (var transformationProfileName in transformationProfileNames)
                {
                    string sourceFile = configurationFile.FullName;
                    string profile = transformationProfileName;

                    configurationFileTransformer.Verify(t => t.Transform(sourceFile, It.Is<string>(s => s.Contains(profile)), sourceFile), Times.Once());
                }
            }
        }

        [Test]
        public void TransformConfigurationFiles_ConfigurationFilesFound_TransformationFilesAreDeletedAfterTheTransformation()
        {
            // Arrange
            string baseDirectoryPath = Environment.CurrentDirectory;
            var transformationProfileNames = new[] { "PROD" };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // prepare filesystem accessor
            var configurationFiles = new List<FileInfo>
                {
                    new FileInfo(Path.Combine(baseDirectoryPath, "websites", "website.A", "web.config")),
                    new FileInfo(Path.Combine(baseDirectoryPath, "webapplications", "webapp", "web.config")),
                    new FileInfo(Path.Combine(baseDirectoryPath, "applications", "some-app", "app.config"))
                };
            filesystemAccessor.Setup(f => f.GetAllFiles(It.Is<string>(folder => folder.StartsWith(baseDirectoryPath)))).Returns(configurationFiles);

            var transformationFileWebsite = new FileInfo(Path.Combine(baseDirectoryPath, "websites", "website.A", "web.PROD.config"));
            filesystemAccessor.Setup(f => f.GetFiles(It.Is<string>(folder => folder.StartsWith(Path.Combine(baseDirectoryPath, "websites", "website.A"))))).
                Returns(new List<FileInfo> { transformationFileWebsite });

            var transformationFileWebApp = new FileInfo(Path.Combine(baseDirectoryPath, "webapplications", "webapp", "web.PROD.config"));
            filesystemAccessor.Setup(f => f.GetFiles(It.Is<string>(folder => folder.StartsWith(Path.Combine(baseDirectoryPath, "webapplications", "webapp"))))).
                Returns(new List<FileInfo> { transformationFileWebApp });

            var transformationFileApp = new FileInfo(Path.Combine(baseDirectoryPath, "applications", "some-app", "app.PROD.config"));
            filesystemAccessor.Setup(f => f.GetFiles(It.Is<string>(folder => folder.StartsWith(Path.Combine(baseDirectoryPath, "applications", "some-app"))))).
                Returns(new List<FileInfo> { transformationFileApp });

            var transformationFiles = new List<FileInfo> { transformationFileWebsite, transformationFileWebApp, transformationFileApp };

            var configurationFileTransformationService = new ConfigurationFileTransformationService(
                filesystemAccessor.Object, configurationFileTransformer.Object);

            // Act
            configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert
            foreach (var transformationFile in transformationFiles)
            {
                string filePath = transformationFile.FullName;
                filesystemAccessor.Verify(f => f.DeleteFile(It.Is<string>(s => s.Equals(filePath))), Times.Once());
            }
        }

        #endregion
    }
}