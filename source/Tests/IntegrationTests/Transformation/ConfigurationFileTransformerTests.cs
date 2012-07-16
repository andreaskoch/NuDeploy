using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Transformation
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