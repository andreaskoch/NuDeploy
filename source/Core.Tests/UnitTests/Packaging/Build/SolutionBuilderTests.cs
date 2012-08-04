using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Services.Packaging.Build;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging.Build
{
    [TestFixture]
    public class SolutionBuilderTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var buildParameterProvider = new Mock<IBuildPropertyProvider>();

            // Act
            var solutionBuilder = new SolutionBuilder(buildFolderPathProvider.Object, buildParameterProvider.Object);

            // Assert
            Assert.IsNotNull(solutionBuilder);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildFolderPathProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var buildParameterProvider = new Mock<IBuildPropertyProvider>();

            // Act
            new SolutionBuilder(null, buildParameterProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildParameterProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();

            // Act
            new SolutionBuilder(buildFolderPathProvider.Object, null);
        }

        #endregion

        #region Build

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Build_SolutionPathIsInvalid_ArgumentExceptionIsThrown(string solutionPath)
        {
            // Arrange
            var buildConfiguration = "Debug";
            var additionalBuildProperties = new Dictionary<string, string>();

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var buildParameterProvider = new Mock<IBuildPropertyProvider>();
            var solutionBuilder = new SolutionBuilder(buildFolderPathProvider.Object, buildParameterProvider.Object);

            // Act
            solutionBuilder.Build(solutionPath, buildConfiguration, additionalBuildProperties);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Build_BuildConfigurationIsInvalid_ArgumentExceptionIsThrown(string buildConfiguration)
        {
            // Arrange
            string solutionPath = @"C:\dev\some-project\project.sln";
            var additionalBuildProperties = new Dictionary<string, string>();

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var buildParameterProvider = new Mock<IBuildPropertyProvider>();
            var solutionBuilder = new SolutionBuilder(buildFolderPathProvider.Object, buildParameterProvider.Object);

            // Act
            solutionBuilder.Build(solutionPath, buildConfiguration, additionalBuildProperties);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Build_AdditionalBuildPropertiesParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string solutionPath = @"C:\dev\some-project\project.sln";
            string buildConfiguration = "Debug";
            Dictionary<string, string> additionalBuildProperties = null;

            var buildFolderPathProvider = new Mock<IBuildFolderPathProvider>();
            var buildParameterProvider = new Mock<IBuildPropertyProvider>();
            var solutionBuilder = new SolutionBuilder(buildFolderPathProvider.Object, buildParameterProvider.Object);

            // Act
            solutionBuilder.Build(solutionPath, buildConfiguration, additionalBuildProperties);
        }

        #endregion
    }
}