using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Status;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Installation
{
    [TestFixture]
    public class PackageInstallerTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Assert
            Assert.IsNotNull(packageInstaller);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                null,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                null,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                null,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                null,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                null,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryBrowserParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                null,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConfigurationFileTransformerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                null,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellExecutorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                null);
        }

        #endregion

        #region Install

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Install_PackageIdParameterIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames);
        }

        [Test]
        public void Install_NoRepositoriesConfigured_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>();
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Uninstall

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Uninstall_PackageIdIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            packageInstaller.Uninstall(packageId);
        }

        [Test]
        public void Uninstall_PackageIsNotInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageInstaller.Uninstall(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_UninstallScriptIsNotFound_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(false);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageInstaller.Uninstall(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(false);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageInstaller.Uninstall(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_PackageIsRemovedFromConfiguration()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            packageInstaller.Uninstall(packageId);

            // Assert
            packageConfigurationAccessor.Verify(p => p.Remove(packageId), Times.Once());
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_PackageDirectoryIsRemoved()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            packageInstaller.Uninstall(packageId);

            // Assert
            filesystemAccessor.Verify(f => f.DeleteDirectory(package.Folder), Times.Once());
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageInstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageInstaller.Uninstall(packageId);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}