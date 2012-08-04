using System;

using Moq;

using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Transformation
{
    [TestFixture]
    public class PackageConfigurationTransformationServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // Act
            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Assert
            Assert.IsNotNull(packageConfigurationTransformationService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConfigurationFileTransformerParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Act
            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(null);

            // Assert
            Assert.IsNotNull(packageConfigurationTransformationService);
        }

        #endregion

        #region TransformSystemSettings

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void TransformSystemSettings_PackageFolderParameterIsInvalid_ArgumentExceptionIsThrown(string packageFolder)
        {
            // Arrange
            string[] systemSettingTransformationProfileNames = new string[] { };

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformSystemSettings_SystemSettingTransformationProfileNamesParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string packageFolder = @"C:\temp\Build\Prepackage";
            string[] systemSettingTransformationProfileNames = null;

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);
        }

        [Test]
        public void TransformSystemSettings_NoTransformationProfilesAreSpecified_ResultIsTrue()
        {
            // Arrange
            string packageFolder = @"C:\temp\Build\Prepackage";
            var systemSettingTransformationProfileNames = new string[] { };

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            bool result = packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TransformSystemSettings_TransformIsCalledForEachProfile_TransformationSucceeds()
        {
            // Arrange
            string packageFolder = @"C:\temp\Build\Prepackage";
            var systemSettingTransformationProfileNames = new[] { "profile1", "profile2", "profile3" };

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            configurationFileTransformer.Setup(t => t.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);

            // Assert
            foreach (var systemSettingTransformationProfileName in systemSettingTransformationProfileNames)
            {
                string profileName = systemSettingTransformationProfileName;

                configurationFileTransformer.Verify(
                    t =>
                    t.Transform(
                        It.IsAny<string>(),
                        It.Is<string>(transformationFilePath => transformationFilePath.Contains(profileName)),
                        It.Is<string>(
                            destinationFilePath => destinationFilePath.EndsWith(PackageConfigurationTransformationService.TransformedSystemSettingsFileName))),
                    Times.Once());
            }
        }

        [TestCase("profile1")]
        [TestCase("profile2")]
        [TestCase("profile3")]
        public void TransformSystemSettings_TransformationFails_ResultIsFalse(string profileWhichFailsTheTransformation)
        {
            // Arrange
            string packageFolder = @"C:\temp\Build\Prepackage";
            var systemSettingTransformationProfileNames = new[] { "profile1", "profile2", "profile3" };

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            configurationFileTransformer.Setup(t => t.Transform(It.IsAny<string>(), It.Is<string>(transformationFilePath => transformationFilePath.Contains(profileWhichFailsTheTransformation) == false), It.IsAny<string>())).Returns(true);
            configurationFileTransformer.Setup(t => t.Transform(It.IsAny<string>(), It.Is<string>(transformationFilePath => transformationFilePath.Contains(profileWhichFailsTheTransformation) == true), It.IsAny<string>())).Returns(false);

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            bool result = packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TransformSystemSettings_TransformationSucceeds_ResultIsTrue()
        {
            // Arrange
            string packageFolder = @"C:\temp\Build\Prepackage";
            var systemSettingTransformationProfileNames = new[] { "profile1", "profile2", "profile3" };

            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            configurationFileTransformer.Setup(t => t.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var packageConfigurationTransformationService = new PackageConfigurationTransformationService(configurationFileTransformer.Object);

            // Act
            bool result = packageConfigurationTransformationService.TransformSystemSettings(packageFolder, systemSettingTransformationProfileNames);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}