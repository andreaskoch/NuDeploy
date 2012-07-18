using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Status;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Cleanup
{
    [TestFixture]
    public class CleanupServiceTests
    {
        #region constructor
        
        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Assert
            Assert.IsNotNull(cleanupService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new CleanupService(null, installationStatusProvider.Object, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new CleanupService(userInterface.Object, null, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            new CleanupService(userInterface.Object, installationStatusProvider.Object, null);
        }

        #endregion

        #region Cleanup (parameterless)

        [Test]
        public void Cleanup_InstallationStatusProviderReturnNoPackages_ResultIsFalse()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(new List<NuDeployPackageInfo>());

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Cleanup_InstallationStatusProviderReturnPackages_AllPackagesAreInstalled_ResultIsFalse()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", true),
                    this.GetPackage("Package.B", true),
                    this.GetPackage("Package.C", true),
                };
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Cleanup_InstallationStatusProviderReturnThreePackages_OneIsInstalled_TwoAreNot_DeleteDirectoryIsCalledForEachLegacyPackage_ResultIsTrue()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var installedPackages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", true)
                };

            var legacyPackages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.B", false),
                    this.GetPackage("Package.C", false),
                };

            var packages = installedPackages.Union(legacyPackages).ToList();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup();

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

            Assert.IsTrue(result);
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
            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            cleanupService.Cleanup(packageId);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnNoPackages_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(new List<NuDeployPackageInfo>());

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnNoPackagesWhichMatchTheSuppliedName_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.D";

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", true),
                    this.GetPackage("Package.B", true),
                    this.GetPackage("Package.C", true),
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnPackages_AllPackageVersionsAreInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var packages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", true, 1),
                    this.GetPackage("Package.A", true, 2),
                    this.GetPackage("Package.A", true, 3),
                };
            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Cleanup_Parameterized_InstallationStatusProviderReturnThreePackages_OneIsInstalled_TwoAreNot_DeleteDirectoryIsCalledForEachLegacyPackage_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";

            var userInterface = new Mock<IUserInterface>();
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var installedPackages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", true, 9)
                };

            var legacyPackages = new List<NuDeployPackageInfo>
                {
                    this.GetPackage("Package.A", false, 8),
                    this.GetPackage("Package.A", false, 7),
                };

            var packages = installedPackages.Union(legacyPackages).ToList();

            installationStatusProvider.Setup(i => i.GetPackageInfo()).Returns(packages);

            var cleanupService = new CleanupService(userInterface.Object, installationStatusProvider.Object, filesystemAccessor.Object);

            // Act
            bool result = cleanupService.Cleanup(packageId);

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

            Assert.IsTrue(result);
        }

        #endregion

        #region utility methods

        private NuDeployPackageInfo GetPackage(string id, bool isInstalled, int revision = 0)
        {
            var version = new SemanticVersion(1, 0, 0, revision);
            string folderName = string.Format("{0}.{1}", id, version);
            string folderPath = Path.Combine(Environment.CurrentDirectory, folderName);

            return new NuDeployPackageInfo { Id = id, IsInstalled = isInstalled, Folder = folderPath, Version = version };
        }

        #endregion
    }
}