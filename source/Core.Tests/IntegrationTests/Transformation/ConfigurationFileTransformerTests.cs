using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Transformation
{
    [TestFixture]
    public class ConfigurationFileTransformerTests
    {
        private const string SampleFileFolder = "samples";

        private IFilesystemAccessor filesystemAccessor;

        private IConfigurationFileTransformer configurationFileTransformer;

        [TestFixtureSetUp]
        public void Setup()
        {
            IUserInterface userInterface = new Mock<IUserInterface>().Object;
            this.filesystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            this.configurationFileTransformer = new ConfigurationFileTransformer(userInterface, this.filesystemAccessor);
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

        #region Transform

        [TestCase("Sample-1-SetAttribute.source", "Sample-1-SetAttribute.transformation", "SetSingleAttribute")]
        [TestCase("Sample-2-SetAttributes.source", "Sample-2-SetAttributes.transformation", "SetMultipleAttributes")]
        [TestCase("Sample-3-Remove.source", "Sample-3-Remove.transformation", "Remove")]
        [TestCase("Sample-4-Insert.source", "Sample-4-Insert.transformation", "Insert")]
        [TestCase("Sample-5-XPath.source", "Sample-5-XPath.transformation", "XPath")]
        public void Transform_SetAttributes_ResultIsTrue_DestinationFileIsCreated(string sourceFileName, string transformationFileName, string destinationPreFix)
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath(sourceFileName);
            var transformationFilePath = this.GetSourceFilePath(transformationFileName);
            var destinationFilePath = this.GetTempFolderPath(destinationPreFix + "-destination.config");

            string sourceFileContent = File.ReadAllText(sourceFilePath);

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            string destinationFileContent = File.ReadAllText(destinationFilePath);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(destinationFilePath));
            Assert.AreNotEqual(sourceFileContent, destinationFileContent);
        }

        [Test]
        public void Transform_DestinationFileExist_ResultIsTrue_DestinationFileIsOverriden()
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.source");
            var transformationFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.transformation");
            var destinationFilePath = this.GetTempFolderPath("destination.config");

            string destinationFileContent = Guid.NewGuid().ToString();
            File.WriteAllText(destinationFilePath, destinationFileContent);

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            string newDestinationFileContent = File.ReadAllText(destinationFilePath);
            Assert.IsTrue(result);
            Assert.AreNotEqual(destinationFileContent, newDestinationFileContent);
        }

        [Test]
        public void Transform_TransformationFails_ResultIsFalse_DestinationFileIsNotCreated()
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath("Sample-6-Invalid-XPath.source");
            var transformationFilePath = this.GetSourceFilePath("Sample-6-Invalid-XPath.transformation");
            var destinationFilePath = this.GetTempFolderPath("destination.config");

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(File.Exists(destinationFilePath));
        }

        [Test]
        public void Transform_ResultIsTrue_DestinationFilePathIsCreated()
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.source");
            var transformationFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.transformation");
            var destinationFilePath = this.GetTempFolderPath(Path.Combine("some", "very", "nested", "folder", "structure", "destination.config"));

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(destinationFilePath));
        }

        [Test]
        public void Transform_DestinationFileIsBeingRead_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.source");
            var transformationFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.transformation");
            var destinationFilePath = this.GetTempFolderPath("destination.config");

            File.WriteAllText(destinationFilePath, Guid.NewGuid().ToString());
            var reader = new StreamReader(destinationFilePath);

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            reader.Close();
            Assert.IsFalse(result);
        }

        [Test]
        public void Transform_DestinationFileIsBeingWrittenTo_ResultIsFalse()
        {
            // Arrange
            string sourceFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.source");
            var transformationFilePath = this.GetSourceFilePath("Sample-1-SetAttribute.transformation");
            var destinationFilePath = this.GetTempFolderPath("destination.config");

            File.WriteAllText(destinationFilePath, Guid.NewGuid().ToString());
            var writer = new StreamWriter(destinationFilePath);

            // Act
            bool result = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath);

            // Assert
            writer.Close();
            Assert.IsFalse(result);
        }

        #endregion

        #region Utility Methods

        private string GetSourceFilePath(string fileName)
        {
            return Path.Combine("IntegrationTests", "Transformation", fileName);
        }

        private string GetTempFolderPath(params string[] relativeFilePath)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, SampleFileFolder, Path.Combine(relativeFilePath));
            return filePath;
        }

        #endregion
    }
}