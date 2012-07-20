using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.Status;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Status
{
    [TestFixture]
    public class InstallationLogicProviderTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            // Act
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Assert
            Assert.IsNotNull(installationLogicProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusProviderParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Act
            new InstallationLogicProvider(null);
        }

        #endregion

        #region IsInstallRequired

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void IsInstallRequired_PackageIdIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsInstallRequired_NewPackageVersionIsNull_ArgumentExceptionIsThrown()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion newPackageVersion = null;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);
        }

        [Test]
        public void IsInstallRequired_ForceInstallationIsSetToTrue_ResultIsTrue_NopMatterWhatTheOtherParametersAre()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            bool forceInstallation = true;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsInstallRequired_NoPackagesAreInstalled_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>();
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsInstallRequired_PackageIsNotYetInstalled_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, false)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsInstallRequired_PackageIsAlreadyInstalled_CurrentVersionEqualsNewVersion_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var installedPackage = TestUtilities.GetPackage(packageId, true);
            installedPackage.Version = newPackageVersion;

            var packages = new List<NuDeployPackageInfo>
                {
                    installedPackage
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsInstallRequired_PackageIsAlreadyInstalled_CurrentVersionIsGreaterNewVersion_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 5);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true, 6)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsInstallRequired_PackageIsAlreadyInstalled_CurrentVersionIsOlderThanNewVersion_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 9);
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true, 7)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsInstallRequired(packageId, newPackageVersion, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region IsUninstallRequired

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void IsUninstallRequired_PackageIdIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsUninstallRequired_NewPackageVersionParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string packageId = "Package.A";
            SemanticVersion newPackageVersion = null;
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);
        }

        [Test]
        public void IsUninstallRequired_DeploymentTypeIsUpdate_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Update;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();
            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUninstallRequired_NoPackagesAreInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>();
            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);            

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUninstallRequired_PackageIsNotInstalled_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, false)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUninstallRequired_PackageIsInstalled_InstallationIsForced_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = true;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsUninstallRequired_PackageIsInstalled_CurrentVersionEqualsNewVersion_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 0);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUninstallRequired_PackageIsInstalled_CurrentVersionIsGreaterNewVersion_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 4);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true, 6)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsUninstallRequired_PackageIsInstalled_CurrentVersionIsOlderThanNewVersion_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            var newPackageVersion = new SemanticVersion(1, 0, 0, 8);
            var deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            var installationStatusProvider = new Mock<IInstallationStatusProvider>();

            var packages = new List<NuDeployPackageInfo>
                {
                    TestUtilities.GetPackage(packageId, true, 1)
                };

            installationStatusProvider.Setup(i => i.GetPackageInfo(packageId)).Returns(packages);

            var installationLogicProvider = new InstallationLogicProvider(installationStatusProvider.Object);

            // Act
            bool result = installationLogicProvider.IsUninstallRequired(packageId, newPackageVersion, deploymentType, forceInstallation);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}