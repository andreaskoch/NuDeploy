using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class PublishCommandTests
    {
        private LoggingUserInterface loggingUserInterface;

        [SetUp]
        public void BeforeEachTest()
        {
            this.loggingUserInterface = new LoggingUserInterface();
        }

        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // Assert
            Assert.IsNotNull(publishCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(publishCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PublishCommand(null, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishingServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            // Act
            new PublishCommand(userInterface.Object, null);
        }

        #endregion

        #region Execute (No Package Path)

        [Test]
        public void Execute_PackagePathParameterIsNotSet_PublishIsNotExecuted()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_PackagePathParameterIsNotSet_ResultIsFalse()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments = new Dictionary<string, string>();

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_PackagePathParameterIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(this.loggingUserInterface.UserInterfaceOutput));
        }

        #endregion

        #region Execute (Invalid Package Path)

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PackagePathParameterIsInvalid_PublishIsNotExecuted(string packagePath)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            publishCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PackagePathParameterIsInvalid_ResultIsFalse(string packagePath)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PackagePathParameterIsInvalid_MessageIsWrittenToUserInterface(string packagePath)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            publishCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(this.loggingUserInterface.UserInterfaceOutput));
        }

        #endregion

        #region Execute (No PublishConfiguration)

        [Test]
        public void Execute_PublishConfigurationParameterIsNotSet_PublishIsNotExecuted()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            publishCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_PublishConfigurationParameterIsNotSet_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_PublishConfigurationParameterIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);

            // Act
            publishCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(this.loggingUserInterface.UserInterfaceOutput));
        }

        #endregion

        #region Execute (Invalid PublishConfiguration)

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PublishConfigurationParameterIsInvalid_PublishIsNotExecuted(string publishConfiguration)
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            publishCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PublishConfigurationParameterIsInvalid_ResultIsFalse(string publishConfiguration)
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PublishConfigurationParameterIsInvalid_MessageIsWrittenToUserInterface(string publishConfiguration)
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";

            var publishingService = new Mock<IPublishingService>();

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            publishCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(this.loggingUserInterface.UserInterfaceOutput));
        }

        #endregion

        #region Execute (All Parameters Valid, Publish Fails)

        [Test]
        public void Execute_AllParametersValid_PublishFails_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";
            string publishConfiguration = "Nuget Gallery";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            // prepare publishing service
            publishingService.Setup(p => p.PublishPackage(packagePath, publishConfiguration)).Returns(false);

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_AllParametersValid_PublishFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";
            string publishConfiguration = "Nuget Gallery";

            var publishingService = new Mock<IPublishingService>();

            // prepare publishing service
            publishingService.Setup(p => p.PublishPackage(packagePath, publishConfiguration)).Returns(false);

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsFalse(result);
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath));
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfiguration));
        }

        #endregion

        #region Execute (All Parameters Valid, Publish Succeeds)

        [Test]
        public void Execute_AllParametersValid_PublishSucceeds_ResultIsTrue()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";
            string publishConfiguration = "Nuget Gallery";

            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            // prepare publishing service
            publishingService.Setup(p => p.PublishPackage(packagePath, publishConfiguration)).Returns(true);

            var publishCommand = new PublishCommand(userInterface.Object, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_AllParametersValid_PublishSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packagePath = @"C:\local-repository\APackage.1.0.0.nupkg";
            string publishConfiguration = "Nuget Gallery";

            var publishingService = new Mock<IPublishingService>();

            // prepare publishing service
            publishingService.Setup(p => p.PublishPackage(packagePath, publishConfiguration)).Returns(true);

            var publishCommand = new PublishCommand(this.loggingUserInterface.UserInterface, publishingService.Object);

            // prepare arguments
            publishCommand.Arguments.Add(PublishCommand.ArgumentNameNugetPackagePath, packagePath);
            publishCommand.Arguments.Add(PublishCommand.ArgumentNamePublishConfigurationName, publishConfiguration);

            // Act
            var result = publishCommand.Execute();

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath));
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfiguration));
        }

        #endregion
    }
}