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

        public const string UninstallPowerShellScriptName = "Remove.ps1";

        public const string SystemSettingsFileName = "systemsettings.xml";

        public const string SystemSettingsFolder = "tools";

        public const string SystemSettingsTransformationFilenameTemplate = "systemsettings.transformation.{0}.xml";

        public const string TransformedSystemSettingsFileName = SystemSettingsFileName + ".transformed";

        public const string DeploymentTypeFull = "full";

        public const string DeploymentTypeUpdate = "update";

        public const string DeploymentTypeDefault = DeploymentTypeFull;

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IConfigurationFileTransformer configurationFileTransformer;

        private readonly IPowerShellExecutor powerShellExecutor;

        public PackageInstaller(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageConfigurationAccessor packageConfigurationAccessor, IPackageRepositoryBrowser packageRepositoryBrowser, IConfigurationFileTransformer configurationFileTransformer, IPowerShellExecutor powerShellExecutor)
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

            if (installationStatusProvider == null)
            {
                throw new ArgumentNullException("installationStatusProvider");
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

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.configurationFileTransformer = configurationFileTransformer;
            this.powerShellExecutor = powerShellExecutor;
        }

        public bool Install(string packageId, string deploymentType, bool forceInstallation, string[] systemSettingTransformationProfileNames)
        {
            // check package source configuration
            if (this.packageRepositoryBrowser.RepositoryConfigurations == null || this.packageRepositoryBrowser.RepositoryConfigurations.Count() == 0)
            {
                this.userInterface.WriteLine(Resources.PackageInstaller.NoPackageRepositoryConfigurationsAvailable);
                return false;
            }

            // fetch package
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

            // check if package is already installed
            NuDeployPackageInfo packageInfoOfInstalledVersion = this.installationStatusProvider.GetPackageInfo(package.Id).FirstOrDefault(p => p.IsInstalled);
            if (packageInfoOfInstalledVersion != null && deploymentType.Equals(DeploymentTypeFull, StringComparison.OrdinalIgnoreCase))
            {
                if (forceInstallation == false)
                {
                    if (package.Version == packageInfoOfInstalledVersion.Version)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.PackageInstaller.LatestVersionAlreadyInstalledMessageTemplate,
                                packageInfoOfInstalledVersion.Id,
                                packageInfoOfInstalledVersion.Version));

                        return false;
                    }

                    if (package.Version < packageInfoOfInstalledVersion.Version)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.PackageInstaller.NewerVersionAlreadyInstalledMessageTemplate,
                                packageInfoOfInstalledVersion.Id,
                                packageInfoOfInstalledVersion.Version,
                                NuDeployConstants.CommonCommandOptionNameForce));

                        return false;
                    }
                }

                /* installed version is older and must be removed */
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.PackageInstaller.RemovingPreviousVersionMessageTemplate,
                        packageInfoOfInstalledVersion.Id,
                        packageInfoOfInstalledVersion.Folder));

                bool uninstallResult = this.Uninstall(packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Version);
                if (uninstallResult)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PackageInstaller.PackageSuccessfullyRemovedMessageTemplate,
                            packageInfoOfInstalledVersion.Id,
                            packageInfoOfInstalledVersion.Version));
                }
                else
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PackageInstaller.PackageRemovalFailedMessageTemplate,
                            packageInfoOfInstalledVersion.Id,
                            packageInfoOfInstalledVersion.Version));

                    if (forceInstallation == false)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.PackageInstaller.PackageRemovalFailedForceHintMessageTemplate, NuDeployConstants.CommonCommandOptionNameForce));

                        return false;
                    }
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

        public bool Uninstall(string packageId, SemanticVersion version = null)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            // check if package is installed
            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageIsNotInstalledMessageTemplate, packageId));
                return false;
            }

            // find the uninstall script
            string uninstallScriptPath = Path.Combine(installedPackage.Folder, UninstallPowerShellScriptName);
            if (this.filesystemAccessor.FileExists(uninstallScriptPath) == false)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.PackageInstaller.UninstallScriptNotFoundMessageTemplate,
                        UninstallPowerShellScriptName,
                        installedPackage.Id,
                        installedPackage.Version,
                        installedPackage.Folder));

                return false;
            }

            // uninstall
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.StartingUninstallMessageTemplate, installedPackage.Id, installedPackage.Version));
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ExecutingUninstallScriptMessageTemplate, uninstallScriptPath));
            if (!this.powerShellExecutor.ExecuteScript(uninstallScriptPath))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ExecutingUninstallScriptFailedMessageTemplate, uninstallScriptPath));
                return false;
            }

            // update package configuration
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.RemovingPackageFromConfigurationMessageTemplate, installedPackage.Id, installedPackage.Id));
            this.packageConfigurationAccessor.Remove(installedPackage.Id);

            // remove package files
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.DeletingPackageFolderMessageTemplate, installedPackage.Folder));
            this.filesystemAccessor.DeleteDirectory(installedPackage.Folder);

            return true;
        }
    }
}