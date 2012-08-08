using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Transformation.ConfigFiles
{
    [TestFixture]
    public class ConfigurationFileTransformationServiceTests
    {
        private IConfigurationFileTransformationService configurationFileTransformationService;

        private IFilesystemAccessor filesystemAccessor;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.filesystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            IConfigurationFileTransformer configurationFileTransformer = new ConfigurationFileTransformer(this.filesystemAccessor);
            this.configurationFileTransformationService = new ConfigurationFileTransformationService(this.filesystemAccessor, configurationFileTransformer);
        }

        [Test]
        public void TransformConfigurationFiles_TransformationIsPerformed_ResultIsTrue()
        {
            // Arrange
            string baseDirectoryPath = this.GetSampleFolderPath("sample-package-1");
            string targetProfile = "DEV";
            string[] transformationProfileNames = new[] { targetProfile };

            string configFileSearchPattern = "*" + ConfigurationFileTransformationService.ConfigurationFileExtension;
            var allConfigurationFilesBeforeTransformation = Directory.GetFiles(baseDirectoryPath, configFileSearchPattern, SearchOption.AllDirectories).ToList();

            var appAndWebConfigFiles =
                allConfigurationFilesBeforeTransformation.Where(
                    f => f.EndsWith("web.config", StringComparison.OrdinalIgnoreCase) || f.EndsWith("app.config", StringComparison.OrdinalIgnoreCase)).ToList();

            var configFilesAndTheirContent = new Dictionary<string, string>();
            foreach (var appAndWebConfigFile in appAndWebConfigFiles)
            {
                configFilesAndTheirContent.Add(appAndWebConfigFile, File.ReadAllText(appAndWebConfigFile));
            }

            // Act
            var result = this.configurationFileTransformationService.TransformConfigurationFiles(baseDirectoryPath, transformationProfileNames);

            // Assert that content has changed
            foreach (var keyValuePair in configFilesAndTheirContent)
            {
                string filePath = keyValuePair.Key;
                string previousFileContent = keyValuePair.Value;
                string newFileContent = File.ReadAllText(filePath);

                Assert.AreNotEqual(previousFileContent, newFileContent);
                Assert.IsTrue(newFileContent.Contains(targetProfile));
            }

            // Assert that transformation files have been deleted
            var allConfigurationFilesAfterTransformation = Directory.GetFiles(baseDirectoryPath, configFileSearchPattern, SearchOption.AllDirectories).ToList();
            Assert.AreNotEqual(allConfigurationFilesBeforeTransformation.Count, allConfigurationFilesAfterTransformation.Count);

            // Assert that result is true
            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        #region utility methods

        private string GetSampleFolderPath(string sampleName)
        {
            return Path.Combine(Environment.CurrentDirectory, "IntegrationTests", "Transformation", "ConfigFiles", sampleName);
        }

        #endregion
    }
}