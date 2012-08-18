using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Installation.Status;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Cleanup
{
    [TestFixture]
    public class CleanupServiceTests
    {
        #region constructor
        
        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Assert
            Assert.IsNotNull(cleanupService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new CleanupService(null, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            new CleanupService(installationStatusProvider.Object, null);
        }

        #endregion

        #region Cleanup (parameterless)

        [Test]
        public void Cleanup_InstallationStatusProviderReturnNoPackages_ResultIsFalse()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(new List<NuDeployPackageInfo>());

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup();

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Cleanup_InstallationStatusProviderReturnPackages_AllPackagesAreInstalled_ResultIsFalse()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", true),
                    TestUtilities.GetPackage("Package.B", true),
                    TestUtilities.GetPackage("Package.C", true),
                };
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup();

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Cleanup_InstallationStatusProviderReturnThreePackages_OneIsInstalled_TwoAreNot_DeleteDirectoryIsCalledForEachLegacyPackage_ResultIsTrue()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DeleteDirectory(It.IsAny<string>())).Returns(true);

            var installedPackages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", true)
                };

            var legacyPackages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.B", false),
                    TestUtilities.GetPackage("Package.C", false),
                };

            var packages = installedPackages.Union(legacyPackages).ToList();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup();

            // Assert
            foreach (var package in installedPackages)
            {
                string packageFolder = package.Folder;
                filesystemAccessor.Verify(f => f.DeleteDirectory(packageFolder), Times.Never());
            }

            foreach (var package in legacyPackages)
            {
                string packageFolder = package.Folder;
                filesystemAccessor.Verify(f => f.DeleteDirectory(packageFolder), Times.Once());
            }

            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        #endregion

        #region Cleanup (parameterized)

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Cleanup_Parameterized_PackageIdIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            cleanupService.Cleanup(packageId);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnNoPackages_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(new List<NuDeployPackageInfo>());

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnNoPackagesWhichMatchTheSuppliedName_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.D";

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", true),
                    TestUtilities.GetPackage("Package.B", true),
                    TestUtilities.GetPackage("Package.C", true),
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnPackages_AllPackageVersionsAreInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", true, 1),
                    TestUtilities.GetPackage("Package.A", true, 2),
                    TestUtilities.GetPackage("Package.A", true, 3),
                };
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnThreePackages_OneIsInstalled_TwoAreNot_DeleteDirectoryIsCalledForEachLegacyPackage_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.DeleteDirectory(It.IsAny<string>())).Returns(true);

            var installedPackages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", true, 9)
                };

            var legacyPackages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage("Package.A", false, 8),
                    TestUtilities.GetPackage("Package.A", false, 7),
                };

            var packages = installedPackages.Union(legacyPackages).ToList();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            var result = cleanupService.Cleanup(packageId);

            // Assert
            foreach (var package in installedPackages)
            {
                string packageFolder = package.Folder;
                filesystemAccessor.Verify(f => f.DeleteDirectory(packageFolder), Times.Never());
            }

            foreach (var package in legacyPackages)
            {
                string packageFolder = package.Folder;
                filesystemAccessor.Verify(f => f.DeleteDirectory(packageFolder), Times.Once());
            }

            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        #endregion
    }
}