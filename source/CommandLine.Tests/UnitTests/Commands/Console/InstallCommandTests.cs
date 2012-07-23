﻿using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class InstallCommandTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            // Act
            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // Assert
            Assert.IsNotNull(installCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            // Act
            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(installCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            // Act
            new InstallCommand(null, packageInstaller.Object, deploymentTypeParser.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageInstallerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            // Act
            new InstallCommand(userInterface.Object, null, deploymentTypeParser.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_DeploymentTypeParserParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();

            // Act
            new InstallCommand(userInterface.Object, packageInstaller.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_NoPackageIdIsSupplied_InstallIsNotExecuted()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments = new Dictionary<string, string>();

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), It.IsAny<bool>(), It.IsAny<string[]>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_NoValidPackageIdIsSupplied_InstallIsNotExecuted(string packageId)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), It.IsAny<bool>(), It.IsAny<string[]>()), Times.Never());
        }

        [Test]
        public void Execute_NoDeploymentTypeSupplied_DeploymentTypeFullIsUsedForInstall()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType expectedDeploymentType = DeploymentType.Full;

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(packageId, expectedDeploymentType, It.IsAny<bool>(), It.IsAny<string[]>()), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("SomeInvalidDeploymentType")]
        public void Execute_InvalidDeploymentTypeSupplied_DeploymentTypeFullIsUsedForInstall(string deploymentType)
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType expectedDeploymentType = DeploymentType.Full;

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentType);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(packageId, expectedDeploymentType, It.IsAny<bool>(), It.IsAny<string[]>()), Times.Once());
        }

        [Test]
        public void Execute_NoSystemSettingTransformationProfilesSupplied_EmptyArrayIsUsedForInstall()
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), It.IsAny<bool>(), It.Is<string[]>(strings => strings.Length == 0)), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_InvalidSystemSettingTransformationProfilesSupplied_EmptyArrayIsUsedForInstall(string systemSettingTransformationProfilesString)
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameSystemSettingTransformationProfiles, systemSettingTransformationProfilesString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), It.IsAny<bool>(), It.Is<string[]>(strings => strings.Length == 0)), Times.Once());
        }

        [Test]
        public void Execute_OneSystemSettingTransformationProfilesSupplied_InstallIsCalledWithOneTransformationProfile()
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";
            string systemSettingTransformationProfilesString = "Profile1";

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameSystemSettingTransformationProfiles, systemSettingTransformationProfilesString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(
                p =>
                p.Install(
                    It.IsAny<string>(),
                    It.IsAny<DeploymentType>(),
                    It.IsAny<bool>(),
                    It.Is<string[]>(strings => strings.Length == 1 && strings.First().Equals(systemSettingTransformationProfilesString))),
                Times.Once());
        }

        [Test]
        public void Execute_TwoSystemSettingTransformationProfilesSupplied_InstallIsCalledWithTwoTransformationProfiles()
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";
            string systemSettingTransformationProfilesString = "Profile1,Profile2";

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameSystemSettingTransformationProfiles, systemSettingTransformationProfilesString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(
                p =>
                p.Install(
                    It.IsAny<string>(),
                    It.IsAny<DeploymentType>(),
                    It.IsAny<bool>(),
                    It.Is<string[]>(strings => strings.Length == 2 && strings.First().Equals("Profile1") && strings.Last().Equals("Profile2"))),
                Times.Once());
        }

        [Test]
        public void Execute_ForceOptionIsNotSupplied_InstallIsCalledWithoutForceOption()
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";

            bool expectedForceOptionValue = false;

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(
                p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), expectedForceOptionValue, It.IsAny<string[]>()), Times.Once());
        }

        [Test]
        public void Execute_ForceOptionIsSet_InstallIsCalledWithForceOption()
        {
            // Arrange
            string packageId = "Package.A";
            string deploymentTypeString = "Full";

            bool expectedForceOptionValue = true;

            var userInterface = new Mock<IUserInterface>();
            var packageInstaller = new Mock<IPackageInstaller>();
            var deploymentTypeParser = new Mock<IDeploymentTypeParser>();

            var installCommand = new InstallCommand(userInterface.Object, packageInstaller.Object, deploymentTypeParser.Object);

            // prepare arguments
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetPackageId, packageId);
            installCommand.Arguments.Add(InstallCommand.ArgumentNameNugetDeploymentType, deploymentTypeString);

            installCommand.Arguments.Add(NuDeployConstants.CommonCommandOptionNameForce, bool.TrueString);

            // Act
            installCommand.Execute();

            // Assert
            packageInstaller.Verify(
                p => p.Install(It.IsAny<string>(), It.IsAny<DeploymentType>(), expectedForceOptionValue, It.IsAny<string[]>()), Times.Once());
        }

        #endregion
    }
}