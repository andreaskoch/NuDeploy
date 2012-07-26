using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
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

        private readonly IUserInterface userInterface;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IPowerShellExecutor powerShellExecutor;

        private readonly IInstallationLogicProvider installationLogicProvider;

        private readonly IPackageUninstaller packageUninstaller;

        private readonly INugetPackageExtractor nugetPackageExtractor;

        private readonly IPackageConfigurationTransformationService packageConfigurationTransformationService;

        private readonly IConfigurationFileTransformationService configurationFileTransformationService;

        public PackageInstaller(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IUserInterface userInterface, IPackageConfigurationAccessor packageConfigurationAccessor, IPackageRepositoryBrowser packageRepositoryBrowser, IPowerShellExecutor powerShellExecutor, IInstallationLogicProvider installationLogicProvider, IPackageUninstaller packageUninstaller, INugetPackageExtractor nugetPackageExtractor, IPackageConfigurationTransformationService packageConfigurationTransformationService, IConfigurationFileTransformationService configurationFileTransformationService)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
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
            this.userInterface = userInterface;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.powerShellExecutor = powerShellExecutor;
            this.installationLogicProvider = installationLogicProvider;
            this.packageUninstaller = packageUninstaller;
            this.nugetPackageExtractor = nugetPackageExtractor;
            this.packageConfigurationTransformationService = packageConfigurationTransformationService;
            this.configurationFileTransformationService = configurationFileTransformationService;
        }

        public bool Install(string packageId, DeploymentType deploymentType, bool forceInstallation, string[] systemSettingTransformationProfileNames)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            if (systemSettingTransformationProfileNames == null)
            {
                throw new ArgumentNullException("systemSettingTransformationProfileNames");
            }

            if (deploymentType == DeploymentType.NotRecognized)
            {
                throw new ArgumentException("deploymentType");
            }

            // check package source configuration
            if (this.packageRepositoryBrowser.RepositoryConfigurations == null || this.packageRepositoryBrowser.RepositoryConfigurations.Count() == 0)
            {
                this.userInterface.WriteLine(Resources.PackageInstaller.NoPackageRepositoryConfigurationsAvailable);
                return false;
            }

            // fetch package from repository
            IPackage package = this.packageRepositoryBrowser.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.PackageInstaller.PackageNotFoundMessageTemplate,
                        packageId,
                        string.Join(", ", this.packageRepositoryBrowser.RepositoryConfigurations.Select(r => r.Url))));

                return false;
            }

            // check if install/update is required
            if (this.installationLogicProvider.IsInstallRequired(packageId, package.Version, forceInstallation) == false)
            {
                return false;
            }

            // uninstall previous version (if required)
            if (this.installationLogicProvider.IsUninstallRequired(packageId, package.Version, deploymentType, forceInstallation))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.RemovingPreviousVersionMessageTemplate, package.Id));

                bool uninstallSucceeded = this.packageUninstaller.Uninstall(package.Id, null);

                this.userInterface.WriteLine(
                        uninstallSucceeded
                        ? string.Format(Resources.PackageInstaller.PackageSuccessfullyRemovedMessageTemplate, package.Id)
                        : string.Format(Resources.PackageInstaller.PackageRemovalFailedMessageTemplate, package.Id));

                if (!uninstallSucceeded && !forceInstallation)
                {
                    // abort installation
                    return false;
                }
            }

            // extract the package
            NuDeployPackageInfo extractedPackage = this.nugetPackageExtractor.Extract(package, this.applicationInformation.StartupFolder);
            if (extractedPackage == null)
            {
                return false;
            }

            // apply system setting transformations
            if (!this.packageConfigurationTransformationService.TransformSystemSettings(extractedPackage.Folder, systemSettingTransformationProfileNames))
            {
                return false;
            }

            // apply configuraton file transformations
            if (!this.configurationFileTransformationService.TransformConfigurationFiles(extractedPackage.Folder, systemSettingTransformationProfileNames))
            {
                return false;
            }

            // execute installation script
            string scriptParameter = string.Format("-{0} {1}", InstallPowerShellScriptDeploymentTypeParameterName, deploymentType);
            string installScriptPath = Path.Combine(extractedPackage.Folder, InstallPowerShellScriptName);

            if (!this.filesystemAccessor.FileExists(installScriptPath))
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.PackageInstaller.InstallScriptNotFoundMessageTemplate,
                        installScriptPath,
                        extractedPackage.Id,
                        extractedPackage.Version,
                        extractedPackage.Folder));

                return false;
            }

            this.userInterface.WriteLine(Resources.PackageInstaller.StartingInstallationPowerShellScriptExecutionMessageTemplate);
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ExecutingInstallScriptMessageTemplate, installScriptPath, scriptParameter));

            if (!this.powerShellExecutor.ExecuteScript(installScriptPath, scriptParameter))
            {
                return false;
            }

            // update package configuration
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.AddingPackageToConfigurationMessageTemplate, package.Id, package.Id));
            this.packageConfigurationAccessor.AddOrUpdate(new PackageInfo { Id = package.Id, Version = package.Version.ToString() });

            return true;
        }
    }
}