using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Status;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Installation
{
    public class PackageUninstallerTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Assert
            Assert.IsNotNull(packageUninstaller);
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
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            packageUninstaller.Uninstall(packageId, version);
        }

        [Test]
        public void Uninstall_PackageIsNotInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_UninstallScriptIsNotFound_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(false);

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(false);

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_PackageIsRemovedFromConfiguration()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            packageUninstaller.Uninstall(packageId, version);

            // Assert
            packageConfigurationAccessor.Verify(p => p.Remove(packageId), Times.Once());
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_PackageDirectoryIsRemoved()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            packageUninstaller.Uninstall(packageId, version);

            // Assert
            filesystemAccessor.Verify(f => f.DeleteDirectory(package.Folder), Times.Once());
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            var packageUninstaller = new PackageUninstaller(
                userInterface.Object,
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            bool result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion         
    }
}