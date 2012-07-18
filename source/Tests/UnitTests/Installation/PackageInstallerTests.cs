using System;

using Moq;

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

        #endregion
    }
}