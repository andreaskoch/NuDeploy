using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Status;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
{
    public class PackageUninstallerTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Assert
            Assert.IsNotNull(packageUninstaller);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageUninstaller(
                null,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageUninstaller(
                installationStatusProvider.Object,
                null,
                filesystemAccessor.Object,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            // Act
            new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                null,
                powerShellExecutor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellExecutorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
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
            SemanticVersion version = null;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageUninstaller = new PackageUninstaller(
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

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            var result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void Uninstall_UninstallSpecificVersion_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = TestUtilities.GetPackage(packageId, true, 3).Version;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, false, 1),
                    TestUtilities.GetPackage(packageId, false, 2),
                    TestUtilities.GetPackage(packageId, true, 3)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            var result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void Uninstall_UninstallScriptIsNotFound_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(false);

            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            var result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(new FailureResult());

            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            var result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void Uninstall_ExecutingUninstallScriptSucceeds_PackageIsRemovedFromConfiguration()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion version = null;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(new SuccessResult());

            packageConfigurationAccessor.Setup(p => p.Remove(It.IsAny<string>())).Returns(new SuccessResult());

            var packageUninstaller = new PackageUninstaller(
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

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(new SuccessResult());

            packageConfigurationAccessor.Setup(p => p.Remove(It.IsAny<string>())).Returns(new SuccessResult());

            var packageUninstaller = new PackageUninstaller(
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

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();

            var package = TestUtilities.GetPackage(packageId, true);
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(new[] { package });

            packageConfigurationAccessor.Setup(p => p.Remove(It.IsAny<string>())).Returns(new SuccessResult());

            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(true);
            filesystemAccessor.Setup(f => f.DeleteDirectory(It.IsAny<string>())).Returns(true);

            powerShellExecutor.Setup(p => p.ExecuteScript(It.Is<string>(s => s.EndsWith(PackageUninstaller.UninstallPowerShellScriptName)))).Returns(new SuccessResult());

            var packageUninstaller = new PackageUninstaller(
                installationStatusProvider.Object,
                packageConfigurationAccessor.Object,
                filesystemAccessor.Object,
                powerShellExecutor.Object);

            // Act
            var result = packageUninstaller.Uninstall(packageId, version);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Success);
        }

        #endregion         
    }
}