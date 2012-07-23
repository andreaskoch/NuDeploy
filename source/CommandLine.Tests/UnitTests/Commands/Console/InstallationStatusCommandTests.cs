using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Status;

using NuGet;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class InstallationStatusCommandTests
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
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            var installationStatusCommand = new InstallationStatusCommand(userInterface.Object, installationStatusProvider.Object);

            // Assert
            Assert.IsNotNull(installationStatusCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            var installationStatusCommand = new InstallationStatusCommand(userInterface.Object, installationStatusProvider.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(installationStatusCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            new InstallationStatusCommand(null, installationStatusProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            // Act
            new InstallationStatusCommand(userInterface.Object, null);
        }

        #endregion

        #region execute

        [Test]
        public void Execute_NoPackageIdIsSupplied_GeneralGetPackageInfoIsCalled()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var installationStatusCommand = new InstallationStatusCommand(this.loggingUserInterface.UserInterface, installationStatusProvider.Object);

            // Act
            installationStatusCommand.Execute();

            // Assert
            installationStatusProvider.Verify(i => i.GetPackageInfo(), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_InvalidPackageIdIsSupplied_GeneralGetPackageInfoIsCalled(string packageId)
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var installationStatusCommand = new InstallationStatusCommand(this.loggingUserInterface.UserInterface, installationStatusProvider.Object);
            
            // prepare command arguments
            installationStatusCommand.Arguments.Add(InstallationStatusCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            installationStatusCommand.Execute();

            // Assert
            installationStatusProvider.Verify(i => i.GetPackageInfo(), Times.Once());
        }

        [Test]
        public void Execute_NoPackageIdSpecified_NoPackagesAreReturned_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>();
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var installationStatusCommand = new InstallationStatusCommand(this.loggingUserInterface.UserInterface, installationStatusProvider.Object);

            // Act
            installationStatusCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PackageIdSpecified_NoPackagesAreReturned_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string packageId = "Package.A";
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>();
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationStatusCommand = new InstallationStatusCommand(this.loggingUserInterface.UserInterface, installationStatusProvider.Object);

            // Act
            installationStatusCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_InstallationStatusProviderReturnsPackages_OneMessageForEachPackageIsWrittenToUserInterface()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    new NuDeployPackageInfo { Id = "Package.A", Version = new SemanticVersion(1, 0, 0, 1), IsInstalled = true },
                    new NuDeployPackageInfo { Id = "Package.B", Version = new SemanticVersion(1, 0, 0, 2), IsInstalled = false },
                };
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var installationStatusCommand = new InstallationStatusCommand(this.loggingUserInterface.UserInterface, installationStatusProvider.Object);

            // Act
            installationStatusCommand.Execute();

            // Assert
            foreach (var packageInfo in packages)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(packageInfo.Id));
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(packageInfo.Version.ToString()));
            }
        }

        #endregion
    }
}