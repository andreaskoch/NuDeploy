using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Installation.Status;
using NuDeploy.Core.Services.Transformation;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
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
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Assert
            Assert.IsNotNull(packageInstaller);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                null,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                null,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                null,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryBrowserParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                null,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellExecutorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                null,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationLogicProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                null,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageUninstallerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                null,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NugetPackageExtractorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                null,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationTransformationServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                null,
                configurationFileTransformationService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConfigurationFileTransformationServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();

            // Act
            new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
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
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Install_DeploymentTypeIsInvalid_ArgumentExceptionIsThrown()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.NotRecognized;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Install_TransformationProfileNamesParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;

            string[] systemSettingTransformationProfileNames = null;

            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Install_BuildConfigurationProfilesParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            string[] systemSettingTransformationProfileNames = new string[] { };

            string[] buildConfigurationProfiles = null;

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);
        }

        [Test]
        public void Install_NoRepositoriesConfigured_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            // configure repository configurations
            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>();
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_FetchPackageFromRepository_PackageIsNotFound_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };

            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            // configure repository browser
            IPackage package = null;
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_InstallationIsNotRequired_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            // configure installation logic
            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_UninstallIsRequired_UninstallFails_InstallationIsNotForced_NextStepIsNotExecuted_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };


            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            // configure uninstaller
            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
            nugetPackageExtractor.Verify(e => e.Extract(package.Object, It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Install_UninstallIsRequired_UninstallFails_InstallationIsForced_NextStepIsExecuted()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;

            bool forceInstallation = true;

            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            // configure uninstaller
            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            nugetPackageExtractor.Verify(e => e.Extract(package.Object, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Install_PackageExtractionFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            // configure package extraction
            NuDeployPackageInfo extractedPackage = null;
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_TransformSystemSettingsFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            // configure system setting transformation
            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).
                Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_TransformConfigurationFilesFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());

            // configure configuration file transformation
            configurationFileTransformationService.Setup(c => c.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_InstallationScriptIsNotFound_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            configurationFileTransformationService.Setup(c => c.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());

            // configure installation script lookup
            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.Contains(PackageInstaller.InstallPowerShellScriptName)))).Returns(false);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_InstallationScriptExecutionFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            configurationFileTransformationService.Setup(c => c.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.Contains(PackageInstaller.InstallPowerShellScriptName)))).Returns(true);

            // configure powershell script execution
            powerShellExecutor.Setup(p => p.ExecuteScript(It.IsAny<string>(), It.IsAny<string>())).Returns(new FailureResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Install_InstallationScriptExecutionSucceeds_PackageConfigurationIsUpdated()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(new SemanticVersion(1, 0, 0, 0));
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            configurationFileTransformationService.Setup(c => c.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.Contains(PackageInstaller.InstallPowerShellScriptName)))).Returns(true);

            // configure powershell script execution
            powerShellExecutor.Setup(p => p.ExecuteScript(It.IsAny<string>(), It.IsAny<string>())).Returns(new SuccessResult());

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            packageConfigurationAccessor.Verify(p => p.AddOrUpdate(It.IsAny<PackageInfo>()), Times.Once());
        }

        [Test]
        public void Install_EverythingSucceeds_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";
            DeploymentType deploymentType = DeploymentType.Full;
            bool forceInstallation = false;
            var systemSettingTransformationProfileNames = new string[] { };
            var buildConfigurationProfiles = new string[] { };

            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            var packageRepositoryBrowser = new Mock<IPackageRepositoryBrowser>();
            var powerShellExecutor = new Mock<IPowerShellExecutor>();
            var installationLogicProvider = new Mock<IInstallationLogicProvider>();
            var packageUninstaller = new Mock<IPackageUninstaller>();
            var nugetPackageExtractor = new Mock<INugetPackageExtractor>();
            var packageConfigurationTransformationService = new Mock<IPackageConfigurationTransformationService>();
            var configurationFileTransformationService = new Mock<IConfigurationFileTransformationService>();

            var sourceRepositoryConfigurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repository", Url = new Uri("http://nuget.example.com/api/v2") }
                };
            packageRepositoryBrowser.Setup(p => p.RepositoryConfigurations).Returns(sourceRepositoryConfigurations);

            var package = new Mock<IPackage>();
            package.Setup(p => p.Id).Returns(packageId);
            package.Setup(p => p.Version).Returns(new SemanticVersion(1, 0, 0, 0));
            packageRepositoryBrowser.Setup(r => r.FindPackage(packageId)).Returns(package.Object);

            installationLogicProvider.Setup(i => i.IsInstallRequired(packageId, It.IsAny<SemanticVersion>(), forceInstallation)).Returns(new SuccessResult());
            installationLogicProvider.Setup(i => i.IsUninstallRequired(packageId, It.IsAny<SemanticVersion>(), deploymentType, forceInstallation)).Returns(new SuccessResult());

            packageUninstaller.Setup(p => p.Uninstall(packageId, It.IsAny<SemanticVersion>())).Returns(new SuccessResult());

            var extractedPackage = new NuDeployPackageInfo { Folder = "Package.A.1.0.0", Id = packageId, IsInstalled = false, Version = new SemanticVersion(1, 0, 0, 0) };
            nugetPackageExtractor.Setup(e => e.Extract(package.Object, It.IsAny<string>())).Returns(extractedPackage);

            packageConfigurationTransformationService.Setup(t => t.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            configurationFileTransformationService.Setup(c => c.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames)).Returns(new SuccessResult());
            filesystemAccessor.Setup(f => f.FileExists(It.Is<string>(s => s.Contains(PackageInstaller.InstallPowerShellScriptName)))).Returns(true);

            // configure powershell script execution
            powerShellExecutor.Setup(p => p.ExecuteScript(It.IsAny<string>(), It.IsAny<string>())).Returns(new SuccessResult());

            packageConfigurationAccessor.Setup(p => p.AddOrUpdate(It.IsAny<PackageInfo>())).Returns(true);

            var packageInstaller = new PackageInstaller(
                applicationInformation,
                filesystemAccessor.Object,
                packageConfigurationAccessor.Object,
                packageRepositoryBrowser.Object,
                powerShellExecutor.Object,
                installationLogicProvider.Object,
                packageUninstaller.Object,
                nugetPackageExtractor.Object,
                packageConfigurationTransformationService.Object,
                configurationFileTransformationService.Object);

            // Act
            var result = packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfiles);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        #endregion
    }
}