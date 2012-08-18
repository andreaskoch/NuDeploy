using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Installation.Status;
using NuDeploy.Core.Services.Transformation;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageInstaller : IPackageInstaller
    {
        public const string InstallPowerShellScriptName = "Deploy.ps1";

        public const string InstallPowerShellScriptDeploymentTypeParameterName = "DeploymentType";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IPowerShellExecutor powerShellExecutor;

        private readonly IInstallationLogicProvider installationLogicProvider;

        private readonly IPackageUninstaller packageUninstaller;

        private readonly INugetPackageExtractor nugetPackageExtractor;

        private readonly IPackageConfigurationTransformationService packageConfigurationTransformationService;

        private readonly IConfigurationFileTransformationService configurationFileTransformationService;

        public PackageInstaller(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IPackageConfigurationAccessor packageConfigurationAccessor, IPackageRepositoryBrowser packageRepositoryBrowser, IPowerShellExecutor powerShellExecutor, IInstallationLogicProvider installationLogicProvider, IPackageUninstaller packageUninstaller, INugetPackageExtractor nugetPackageExtractor, IPackageConfigurationTransformationService packageConfigurationTransformationService, IConfigurationFileTransformationService configurationFileTransformationService)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (packageConfigurationAccessor == null)
            {
                throw new ArgumentNullException("packageConfigurationAccessor");
            }

            if (packageRepositoryBrowser == null)
            {
                throw new ArgumentNullException("packageRepositoryBrowser");
            }

            if (powerShellExecutor == null)
            {
                throw new ArgumentNullException("powerShellExecutor");
            }

            if (installationLogicProvider == null)
            {
                throw new ArgumentNullException("installationLogicProvider");
            }

            if (packageUninstaller == null)
            {
                throw new ArgumentNullException("packageUninstaller");
            }

            if (nugetPackageExtractor == null)
            {
                throw new ArgumentNullException("nugetPackageExtractor");
            }

            if (packageConfigurationTransformationService == null)
            {
                throw new ArgumentNullException("packageConfigurationTransformationService");
            }

            if (configurationFileTransformationService == null)
            {
                throw new ArgumentNullException("configurationFileTransformationService");
            }

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.powerShellExecutor = powerShellExecutor;
            this.installationLogicProvider = installationLogicProvider;
            this.packageUninstaller = packageUninstaller;
            this.nugetPackageExtractor = nugetPackageExtractor;
            this.packageConfigurationTransformationService = packageConfigurationTransformationService;
            this.configurationFileTransformationService = configurationFileTransformationService;
        }

        public IServiceResult Install(string packageId, DeploymentType deploymentType, bool forceInstallation, string[] packageConfigurationProfiles, string[] buildConfigurationProfiles)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            if (packageConfigurationProfiles == null)
            {
                throw new ArgumentNullException("packageConfigurationProfiles");
            }

            if (buildConfigurationProfiles == null)
            {
                throw new ArgumentNullException("buildConfigurationProfiles");
            }

            if (deploymentType == DeploymentType.NotRecognized)
            {
                throw new ArgumentException("deploymentType");
            }

            // check package source configuration
            if (this.packageRepositoryBrowser.RepositoryConfigurations == null || !this.packageRepositoryBrowser.RepositoryConfigurations.Any())
            {
                return new FailureResult(Resources.PackageInstaller.NoPackageRepositoryConfigurationsAvailable);
            }

            // fetch package from repository
            IPackage package = this.packageRepositoryBrowser.FindPackage(packageId);
            if (package == null)
            {
                return new FailureResult(
                    Resources.PackageInstaller.PackageNotFoundMessageTemplate,
                    packageId,
                    string.Join(", ", this.packageRepositoryBrowser.RepositoryConfigurations.Select(r => r.Url)));
            }

            // check if install/update is required
            IServiceResult installRequired = this.installationLogicProvider.IsInstallRequired(packageId, package.Version, forceInstallation);
            if (installRequired.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.PackageInstaller.InstallationIsNotRequiredMessageTemplate, packageId)
                    {
                        InnerResult = installRequired
                    };
            }

            // uninstall previous version (if required)
            IServiceResult uninstallRequired = this.installationLogicProvider.IsUninstallRequired(packageId, package.Version, deploymentType, forceInstallation);
            if (uninstallRequired.Status == ServiceResultType.Success)
            {
                IServiceResult uninstallResult = this.packageUninstaller.Uninstall(package.Id, null);
                if (uninstallResult.Status == ServiceResultType.Failure && !forceInstallation)
                {
                    // abort installation
                    return new FailureResult(Resources.PackageInstaller.PackageRemovalFailedMessageTemplate, package.Id) { InnerResult = uninstallResult };
                }
            }

            // extract the package
            NuDeployPackageInfo extractedPackage = this.nugetPackageExtractor.Extract(package, this.applicationInformation.StartupFolder);
            if (extractedPackage == null)
            {
                return new FailureResult(
                    Resources.PackageInstaller.PackageExtractionFailedMessageTemplate, packageId, this.applicationInformation.StartupFolder);
            }

            // apply system setting transformations
            IServiceResult systemSettingTransformationResult = this.packageConfigurationTransformationService.TransformSystemSettings(
                extractedPackage.Folder, packageConfigurationProfiles);

            if (systemSettingTransformationResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.PackageInstaller.SystemSettingTransformationFailedMessageTemplate, packageId)
                    {
                        InnerResult = systemSettingTransformationResult
                    };
            }

            // apply configuraton file transformations
            IServiceResult configFileTransformationResult = this.configurationFileTransformationService.TransformConfigurationFiles(
                extractedPackage.Folder, buildConfigurationProfiles);

            if (configFileTransformationResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.PackageInstaller.ConfigurationFileTransformationFailedMessageTemplate, packageId)
                    {
                        InnerResult = configFileTransformationResult
                    };
            }

            // locate installation script
            string scriptParameter = string.Format("-{0} {1}", InstallPowerShellScriptDeploymentTypeParameterName, deploymentType);
            string installScriptPath = Path.Combine(extractedPackage.Folder, InstallPowerShellScriptName);

            if (!this.filesystemAccessor.FileExists(installScriptPath))
            {
                return new FailureResult(
                    Resources.PackageInstaller.InstallScriptNotFoundMessageTemplate,
                    installScriptPath,
                    extractedPackage.Id,
                    extractedPackage.Version,
                    extractedPackage.Folder);
            }

            // execute installation script
            IServiceResult powerShellResult = this.powerShellExecutor.ExecuteScript(installScriptPath, scriptParameter);
            if (powerShellResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.PackageInstaller.InstallationScriptExecutionFailedMessageTemplate, installScriptPath)
                    {
                        InnerResult = powerShellResult
                    };
            }

            // update package configuration
            if (this.packageConfigurationAccessor.AddOrUpdate(new PackageInfo { Id = package.Id, Version = package.Version.ToString() }) == false)
            {
                return new FailureResult(Resources.PackageInstaller.PackageCouldNotBeAddedToConfigurationMessageTemplate, packageId, package.Version);
            }

            return new SuccessResult(Resources.PackageInstaller.PackageHasBeenSuccessfullyInstalledMessageTemplate, packageId, package.Version);
        }
    }
}