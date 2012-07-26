using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Packaging
{
    [TestFixture]
    public class SolutionPackagingServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreValid_ObjectIsInstantiated()
        {
            // Arrange
            ISolutionBuilder solutionBuilder = new Mock<ISolutionBuilder>().Object;
            IPrepackagingService prepackagingService = new Mock<IPrepackagingService>().Object;
            IPackagingService packagingService = new Mock<IPackagingService>().Object;

            // Act
            var result = new SolutionPackagingService(solutionBuilder, prepackagingService, packagingService);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SolutionBuilderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISolutionBuilder solutionBuilder = null;
            IPrepackagingService prepackagingService = new Mock<IPrepackagingService>().Object;
            IPackagingService packagingService = new Mock<IPackagingService>().Object;

            // Act
            new SolutionPackagingService(solutionBuilder, prepackagingService, packagingService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PrepackagingServiceParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISolutionBuilder solutionBuilder = new Mock<ISolutionBuilder>().Object;
            IPrepackagingService prepackagingService = null;
            IPackagingService packagingService = new Mock<IPackagingService>().Object;

            // Act
            new SolutionPackagingService(solutionBuilder, prepackagingService, packagingService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackagingServiceParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISolutionBuilder solutionBuilder = new Mock<ISolutionBuilder>().Object;
            IPrepackagingService prepackagingService = new Mock<IPrepackagingService>().Object;
            IPackagingService packagingService = null;

            // Act
            new SolutionPackagingService(solutionBuilder, prepackagingService, packagingService);
        }

        #endregion

        #region PackageSolution

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void PackageSolution_SolutionPathParameterIsInvalid_ResultIsFalse(string solutionPath)
        {
            // Arrange
            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            var packagingServiceMock = new Mock<IPackagingService>();

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void PackageSolution_BuildConfigurationParameterIsInvalid_ResultIsFalse(string buildConfiguration)
        {
            // Arrange
            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            var packagingServiceMock = new Mock<IPackagingService>();

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PackageSolution_BuildPropertiesParameterIsNull_ResultIsFalse()
        {
            // Arrange
            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            var packagingServiceMock = new Mock<IPackagingService>();

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = null;

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PackageSolution_ParametersAreValid_BuildFails_ResultIsFalse()
        {
            // Arrange
            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            solutionBuilderMock.Setup(b => b.Build(solutionPath, buildConfiguration, buildProperties)).Returns(false);

            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            var packagingServiceMock = new Mock<IPackagingService>();

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PackageSolution_ParametersAreValid_BuildSucceeds_PrepackagingFails_ResultIsFalse()
        {
            // Arrange
            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            solutionBuilderMock.Setup(b => b.Build(solutionPath, buildConfiguration, buildProperties)).Returns(true);

            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            prepackagingServiceMock.Setup(p => p.Prepackage()).Returns(false);

            var packagingServiceMock = new Mock<IPackagingService>();

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PackageSolution_ParametersAreValid_BuildSucceeds_PrepackagingSucceeds_PackaginFails_ResultIsFalse()
        {
            // Arrange
            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            solutionBuilderMock.Setup(b => b.Build(solutionPath, buildConfiguration, buildProperties)).Returns(true);

            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            prepackagingServiceMock.Setup(p => p.Prepackage()).Returns(true);

            var packagingServiceMock = new Mock<IPackagingService>();
            packagingServiceMock.Setup(p => p.Package()).Returns(false);

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PackageSolution_ParametersAreValid_BuildSucceeds_PrepackagingSucceeds_PackaginSucceeds_ResultIsTrue()
        {
            // Arrange
            string solutionPath = @"C:\dev\someproject\some-solution.sln";
            string buildConfiguration = "Debug";
            IEnumerable<KeyValuePair<string, string>> buildProperties = new Dictionary<string, string>();

            var solutionBuilderMock = new Mock<ISolutionBuilder>();
            solutionBuilderMock.Setup(b => b.Build(solutionPath, buildConfiguration, buildProperties)).Returns(true);

            var prepackagingServiceMock = new Mock<IPrepackagingService>();
            prepackagingServiceMock.Setup(p => p.Prepackage()).Returns(true);

            var packagingServiceMock = new Mock<IPackagingService>();
            packagingServiceMock.Setup(p => p.Package()).Returns(true);

            var solutionPackagingService = new SolutionPackagingService(solutionBuilderMock.Object, prepackagingServiceMock.Object, packagingServiceMock.Object);

            // Act
            bool result = solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}