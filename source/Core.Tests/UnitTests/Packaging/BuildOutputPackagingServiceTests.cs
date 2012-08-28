using System;

using Moq;

using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Packaging
{
    [TestFixture]
    public class BuildOutputPackagingServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var prepackagingService = new Mock<IPrepackagingService>();
            var packagingService = new Mock<IPackagingService>();

            // Act
            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Assert
            Assert.IsNotNull(buildOutputPackagingService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PrepackagingServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packagingService = new Mock<IPackagingService>();

            // Act
            new BuildOutputPackagingService(null, packagingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackagingServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var prepackagingService = new Mock<IPrepackagingService>();

            // Act
            new BuildOutputPackagingService(prepackagingService.Object, null);
        }

        #endregion

        #region Package

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Package_BuildOutputFolderPathParameterIsInvalid_FailureResultIsReturned(string buildOutputFolderPath)
        {
            // Arrange
            var prepackagingService = new Mock<IPrepackagingService>();
            var packagingService = new Mock<IPackagingService>();
            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Package_PrepackagingFails_FailureResultIsReturned()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new FailureResult());

            var packagingService = new Mock<IPackagingService>();
            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Package_PrepackagingFails_FailureResultContainsBuildOutputFolderPath()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new FailureResult());

            var packagingService = new Mock<IPackagingService>();
            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.IsTrue(result.Message.Contains(buildOutputFolderPath));
        }

        [Test]
        public void Package_PackagingFails_FailureResultIsReturned()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new SuccessResult());

            var packagingService = new Mock<IPackagingService>();
            packagingService.Setup(p => p.Package()).Returns(new FailureResult());

            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Package_PackagingFails_FailureResultContainsBuildOutputFolderPath()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new SuccessResult());

            var packagingService = new Mock<IPackagingService>();
            packagingService.Setup(p => p.Package()).Returns(new FailureResult());

            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.IsTrue(result.Message.Contains(buildOutputFolderPath));
        }

        [Test]
        public void Package_PackagingSucceeds_SuccessResultIsReturned()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new SuccessResult());

            var packagingService = new Mock<IPackagingService>();
            packagingService.Setup(p => p.Package()).Returns(new SuccessResult());

            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        [Test]
        public void Package_PackagingSucceeds_SuccessResultContainsBuildOutputFolderPath()
        {
            // Arrange
            string buildOutputFolderPath = "C:\\built-output";

            var prepackagingService = new Mock<IPrepackagingService>();
            prepackagingService.Setup(p => p.Prepackage(It.IsAny<string>())).Returns(new SuccessResult());

            var packagingService = new Mock<IPackagingService>();
            packagingService.Setup(p => p.Package()).Returns(new SuccessResult());

            var buildOutputPackagingService = new BuildOutputPackagingService(prepackagingService.Object, packagingService.Object);

            // Act
            var result = buildOutputPackagingService.Package(buildOutputFolderPath);

            // Assert
            Assert.IsTrue(result.Message.Contains(buildOutputFolderPath));
        }

        #endregion
    }
}