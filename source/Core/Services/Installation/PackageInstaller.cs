using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Status;
using NuDeploy.Core.Services.Transformation;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageInstaller : IPackageInstaller
    {
        public const string InstallPowerShellScriptName = "Deploy.ps1";

        public const string InstallPowerShellScriptDeploymentTypeParameterName = "DeploymentType";

        public const string SystemSettingsFileName = "systemsettings.xml";

        public const string SystemSettingsFolder = "tools";

        public const string SystemSettingsTransformationFilenameTemplate = "systemsettings.transformation.{0}.xml";

        public const string TransformedSystemSettingsFileName = SystemSettingsFileName + ".transformed";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IUserInterface userInterface;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IConfigurationFileTransformer configurationFileTransformer;

        private readonly IPowerShellExecutor powerShellExecutor;

        private readonly IInstallationLogicProvider installationLogicProvider;

        private readonly IPackageUninstaller packageUninstaller;

        public PackageInstaller(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IUserInterface userInterface, IPackageConfigurationAccessor packageConfigurationAccessor, IPackageRepositoryBrowser packageRepositoryBrowser, IConfigurationFileTransformer configurationFileTransformer, IPowerShellExecutor powerShellExecutor, IInstallationLogicProvider installationLogicProvider, IPackageUninstaller packageUninstaller)
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

            if (configurationFileTransformer == null)
            {
                throw new ArgumentNullException("configurationFileTransformer");
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

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.userInterface = userInterface;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.configurationFileTransformer = configurationFileTransformer;
            this.powerShellExecutor = powerShellExecutor;
            this.installationLogicProvider = installationLogicProvider;
            this.packageUninstaller = packageUninstaller;
        }

        public bool Install(string packageId, DeploymentType deploymentType, bool forceInstallation, string[] systemSettingTransformationProfileNames)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            // check package source configuration
            if (this.packageRepositoryBrowser.RepositoryConfigurations == null || this.packageRepositoryBrowser.RepositoryConfigurations.Count() == 0)
            {
                this.userInterface.WriteLine(Resources.PackageInstaller.NoPackageRepositoryConfigurationsAvailable);
                return false;
            }

            // fetch package from repository
            IPackageRepository packageRepository;
            IPackage package = this.packageRepositoryBrowser.FindPackage(packageId, out packageRepository);
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

            var packageManager = new PackageManager(packageRepository, this.applicationInformation.StartupFolder);
            packageManager.PackageInstalling +=
                (sender, args) =>
                this.userInterface.WriteLine(
                    string.Format(Resources.PackageInstaller.DownloadingPackageMessageTemplate, args.Package.Id, args.Package.Version, args.InstallPath));

            packageManager.PackageInstalled += (sender, args) =>
            {
                string packageFolder = args.InstallPath;
                string installScriptPath = Path.Combine(packageFolder, InstallPowerShellScriptName);

                this.userInterface.WriteLine(
                    string.Format(Resources.PackageInstaller.PackageDownloadedMessageTemplate, args.Package.Id, args.Package.Version, packageFolder));

                if (this.filesystemAccessor.FileExists(installScriptPath) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PackageInstaller.InstallScriptNotFoundMessageTemplate, installScriptPath, package.Id, package.Version, packageFolder));

                    return;
                }

                // apply transformations on the system settings before installation
                if (systemSettingTransformationProfileNames != null && systemSettingTransformationProfileNames.Length > 0)
                {
                    string sourceFileFolder = Path.Combine(packageFolder, SystemSettingsFolder);
                    string sourceFilePath = Path.Combine(sourceFileFolder, SystemSettingsFileName);

                    foreach (var systemSettingTransformationProfileName in systemSettingTransformationProfileNames)
                    {
                        this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ApplyingSystemSettingsTransformationProfileMessageTemplate, systemSettingTransformationProfileName));

                        string transformationFilename = string.Format(SystemSettingsTransformationFilenameTemplate, systemSettingTransformationProfileName);
                        string transformationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, transformationFilename);
                        string destinationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, TransformedSystemSettingsFileName);

                        if (this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath))
                        {
                            this.userInterface.WriteLine(Resources.PackageInstaller.SystemSettingTransformationSucceededMessage);

                            // make the transformed file the source for the next transformation
                            sourceFilePath = destinationFilePath;
                        }
                        else
                        {
                            this.userInterface.WriteLine(Resources.PackageInstaller.SystemSettingTransformationFailedMessage);

                            // abort the transformations
                            break;
                        }
                    }
                }

                // execute installation script
                this.userInterface.WriteLine(Resources.PackageInstaller.StartingInstallationPowerShellScriptExecutionMessageTemplate);

                string scriptParameter = string.Format("-{0} {1}", InstallPowerShellScriptDeploymentTypeParameterName, deploymentType);
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ExecutingInstallScriptMessageTemplate, installScriptPath, scriptParameter));
                this.powerShellExecutor.ExecuteScript(installScriptPath, scriptParameter);

                // update package configuration
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.AddingPackageToConfigurationMessageTemplate, package.Id, package.Id));
                this.packageConfigurationAccessor.AddOrUpdate(new PackageInfo { Id = package.Id, Version = package.Version.ToString() });
            };

            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.StartingInstallationMessageTemplate, package.Id, package.Version));
            packageManager.InstallPackage(package, false, true);
            this.userInterface.WriteLine(Resources.PackageInstaller.InstallationFinishedMessage);

            return true;
        }
    }
}