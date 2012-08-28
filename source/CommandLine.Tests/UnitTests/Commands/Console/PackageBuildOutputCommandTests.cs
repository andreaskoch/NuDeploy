using System;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class PackageBuildOutputCommandTests
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
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // Assert
            Assert.IsNotNull(packageBuildOutputCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(packageBuildOutputCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PackageBuildOutputCommand(null, buildOutputPackagingService.Object, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildOutputPackagingServiceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PackageBuildOutputCommand(userInterface.Object, null, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishingServiceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();

            // Act
            new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, null);
        }

        #endregion

        #region Execute - BuildOutputPath Tests

        [Test]
        public void Execute_BuildOutputPathArgumentIsNotSet_ResultIsFalse()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // Act
            var result = packageBuildOutputCommand.Execute();

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Execute_BuildOutputPathArgumentIsNotSet_PackageSolutionIsNotExecuted()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            buildOutputPackagingService.Verify(s => s.Package(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_BuildOutputPathArgumentIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildOutputPathArgumentIsInvalid_ResultIsFalse(string buildOutputPath)
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare command arguments
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputPath);

            // Act
            var result = packageBuildOutputCommand.Execute();

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildOutputPathArgumentIsInvalid_PackageSolutionIsNotExecuted(string buildOutputPath)
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare command arguments
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            buildOutputPackagingService.Verify(s => s.Package(It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildOutputPathArgumentIsInvalid_MessageIsWrittenToUserInterface(string buildOutputPath)
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();
            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare command arguments
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion

        #region Execute - Package Tests

        [Test]
        public void Execute_PackagingFails_ResultIsFalse()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new FailureResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            var result = packageBuildOutputCommand.Execute();

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Execute_PackagingFails_FailureResultIsWrittenToUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            string failureMessage = "Some error. Yada Yada " + Guid.NewGuid().ToString();
            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new FailureResult(failureMessage));

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(failureMessage),
                "The user interface should contain the failure message \"{0}\" but contains this \"{1}\"",
                failureMessage,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PackagingFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new FailureResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(this.loggingUserInterface.UserInterfaceOutput.Trim()), "The user interface should contain a message");
        }

        [Test]
        public void Execute_PackagingSucceeds_ResultIsTrue()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            var result = packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_PackagingSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput));
        }

        #endregion

        #region Execute Publish Tests

        [Test]
        public void Execute_PublishConfigurationIsNotSet_PackageIsNotPublished()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PublishConfigurationIsInvalid_PackageIsNotPublished(string publishConfigurationName)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_PublishConfigurationIsSet_PublishIsExecuted()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(userInterface.Object, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string publishConfigurationName = "Some Publish Configuration";
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), publishConfigurationName), Times.Once());
        }

        [Test]
        public void Execute_PublishingFails_FailureResultIsWrittenTouUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult());

            string publishingFailureMessage = "Publishing failed. yada yada. " + Guid.NewGuid().ToString();
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new FailureResult(publishingFailureMessage));

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string publishConfigurationName = "Some Publish Configuration";
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(publishingFailureMessage),
                "The user interface should contain the failure message \"{0}\" but contains this \"{1}\"",
                publishingFailureMessage,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PublishingFails_FailureMessageIsWrittenTouUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            string packagePath = @"C:\some-package.nupkg";
            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult { ResultArtefact = packagePath });
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new FailureResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string publishConfigurationName = "Some Publish Configuration";
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath)
                && this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfigurationName),
                "The user interface should contain the package path \"{0}\" and the publish configuration name \"{1}\" but contains this \"{2}\"",
                packagePath,
                publishConfigurationName,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PublishingSucceeds_SuccessMessageIsWrittenTouUserInterface()
        {
            // Arrange
            var buildOutputPackagingService = new Mock<IBuildOutputPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            string packagePath = @"C:\some-package.nupkg";
            buildOutputPackagingService.Setup(b => b.Package(It.IsAny<string>())).Returns(new SuccessResult { ResultArtefact = packagePath });
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new SuccessResult());

            var packageBuildOutputCommand = new PackageBuildOutputCommand(this.loggingUserInterface.UserInterface, buildOutputPackagingService.Object, publishingService.Object);

            // prepare arguments
            string publishConfigurationName = "Some Publish Configuration";
            string buildOutputFolderPath = "C:\\build-output";
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNameBuildOutputFolderPath, buildOutputFolderPath);
            packageBuildOutputCommand.Arguments.Add(PackageBuildOutputCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageBuildOutputCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath)
                && this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfigurationName),
                "The user interface should contain the package path \"{0}\" and the publish configuration name \"{1}\" but contains this \"{2}\"",
                packagePath,
                publishConfigurationName,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion
    }
}