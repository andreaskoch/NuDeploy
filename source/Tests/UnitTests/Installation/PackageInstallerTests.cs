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
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);

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
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                null,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                null,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                null,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                null,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryBrowserParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                null,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConfigurationFileTransformerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                null,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellExecutorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                null,
                installationLogicProvider.Object,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationLogicProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                null,
                packageUninstaller.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageUninstallerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var userInterface = new Mock<IUserInterface>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
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
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);

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
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var configurationFileTransformer = new Mock<IConfigurationFileTransformer>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>();
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                userInterface.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                configurationFileTransformer.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object);

            // Act
            bool result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}