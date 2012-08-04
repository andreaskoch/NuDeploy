using System;
using System.Collections.Generic;

using NuDeploy.Core.Services.Packaging.Build;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Build
{
    [TestFixture]
    public class BuildPropertyProviderTests
    {
        #region GetBuildProperties

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBuildProperties_BuildConfigurationParametersIsInvalid_ArgumentExceptionIsThrown(string buildConfiguration)
        {
            // Arrange
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";
            var additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBuildProperties_TargetPlatformParametersIsInvalid_ArgumentExceptionIsThrown(string targetPlatform)
        {
            // Arrange
            string buildConfiguration = "Debug";
            string outputPath = @"C:\temp\build";
            var additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBuildProperties_OutputPathParametersIsInvalid_ArgumentExceptionIsThrown(string outputPath)
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            var additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBuildProperties_AdditionalBuildPropertiesParametersIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";
            Dictionary<string, string> additionalBuildProperties = null;

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);
        }

        [Test]
        public void GetBuildProperties_BuildConfigurationIsIncludedInBuildProperties()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";
            Dictionary<string, string> additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.AreEqual(result[BuildPropertyProvider.BuildPropertyNameBuildConfiguration], buildConfiguration);
        }

        [Test]
        public void GetBuildProperties_TargetPlatformIsIncludedInBuildProperties()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";
            Dictionary<string, string> additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.AreEqual(result[BuildPropertyProvider.BuildPropertyNameTargetPlatform], targetPlatform);
        }

        [Test]
        public void GetBuildProperties_OutputPathIsIncludedInBuildProperties()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";
            Dictionary<string, string> additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.AreEqual(result[BuildPropertyProvider.BuildPropertyNameOutputPath], outputPath);
        }

        [Test]
        public void GetBuildProperties_AdditionalBuildPropertiesAreEmpty_OnlyTheDefaultPropertiesAreReturned()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";

            var additionalBuildProperties = new Dictionary<string, string>();

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.AreEqual(3, result.Keys.Count);
        }

        [Test]
        public void GetBuildProperties_AdditionalBuildPropertiesContainNewValues_NewValuesAreAddedToBuildProperties()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";

            var value1 = new KeyValuePair<string, string>("key1", "value1");
            var value2 = new KeyValuePair<string, string>("key2", "value2");
            var additionalBuildProperties = new[] { value1, value2 };

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.IsTrue(result.ContainsKey(value1.Key));
            Assert.IsTrue(result.ContainsKey(value2.Key));
        }

        [Test]
        public void GetBuildProperties_AdditionalBuildPropertiesContainsDefaultValue_targetPlatform_DefaultValueIsOverridden()
        {
            // Arrange
            string buildConfiguration = "Debug";
            string targetPlatform = "Any CPU";
            string outputPath = @"C:\temp\build";

            var value1 = new KeyValuePair<string, string>(BuildPropertyProvider.BuildPropertyNameTargetPlatform, "new target platform value");
            var additionalBuildProperties = new[] { value1 };

            var buildPropertyProvider = new BuildPropertyProvider();

            // Act
            var result = buildPropertyProvider.GetBuildProperties(buildConfiguration, targetPlatform, outputPath, additionalBuildProperties);

            // Assert
            Assert.AreEqual(result[BuildPropertyProvider.BuildPropertyNameTargetPlatform], value1.Value);
        }

        #endregion
    }
}