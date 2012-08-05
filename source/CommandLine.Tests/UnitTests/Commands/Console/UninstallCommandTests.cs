using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class UninstallCommandTests
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
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            var uninstallCommand = new UninstallCommand(userInterface.Object, packageUninstaller.Object);

            // Assert
            Assert.IsNotNull(uninstallCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            var uninstallCommand = new UninstallCommand(userInterface.Object, packageUninstaller.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(uninstallCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new UninstallCommand(null, packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageUninstallerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            // Act
            new UninstallCommand(userInterface.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_NoPackageIdIsSupplied_InstallIsNotExecuted()
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments = new Dictionary<string, string>();

            // Act
            uninstallCommand.Execute();

            // Assert
            packageUninstaller.Verify(p => p.Uninstall(It.IsAny<string>(), It.IsAny<SemanticVersion>()), Times.Never());
        }

        [Test]
        public void Execute_NoPackageIdIsSupplied_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments = new Dictionary<string, string>();

            // Act
            uninstallCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_NoValidPackageIdIsSupplied_InstallIsNotExecuted(string packageId)
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments.Add(UninstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            uninstallCommand.Execute();

            // Assert
            packageUninstaller.Verify(p => p.Uninstall(It.IsAny<string>(), It.IsAny<SemanticVersion>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_NoValidPackageIdIsSupplied_MessageIsWrittenToUserInterface(string packageId)
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments.Add(UninstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            uninstallCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PackageIdIsValid_UninstallIsExecuted()
        {
            // Arrange
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            string packageId = "Package.A";
            uninstallCommand.Arguments.Add(UninstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            uninstallCommand.Execute();

            // Assert
            packageUninstaller.Verify(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>()), Times.Once());
        }

        [Test]
        public void Execute_UninstallFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packageId = "Package.A";

            var packageUninstaller = new Mock<IPackageUninstaller>();
            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(false);

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments.Add(UninstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            uninstallCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("fail"));
        }

        [Test]
        public void Execute_UninstallSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packageId = "Package.A";

            var packageUninstaller = new Mock<IPackageUninstaller>();
            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(true);

            var uninstallCommand = new UninstallCommand(this.loggingUserInterface.UserInterface, packageUninstaller.Object);

            // prepare arguments
            uninstallCommand.Arguments.Add(UninstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            uninstallCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("succeeded") || this.loggingUserInterface.UserInterfaceOutput.Contains("success"));
        }

        #endregion
    }
}