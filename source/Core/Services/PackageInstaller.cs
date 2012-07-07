using System;
using System.IO;
using System.Linq;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NuGet;

namespace NuDeploy.Core.Services
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

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IConfigurationFileTransformer configurationFileTransformer;

        private readonly PSHost powerShellHost;

        public PackageInstaller(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageConfigurationAccessor packageConfigurationAccessor, IPackageRepositoryBrowser packageRepositoryBrowser, IConfigurationFileTransformer configurationFileTransformer, PSHost powerShellHost)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.configurationFileTransformer = configurationFileTransformer;
            this.powerShellHost = powerShellHost;
        }

        public bool Install(string packageId, string deploymentType, bool forceInstallation, string systemSettingTransformationProfileName)
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
            if (packageInfoOfInstalledVersion != null)
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
                if (string.IsNullOrWhiteSpace(systemSettingTransformationProfileName) == false)
                {
                    this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ApplyingSystemSettingsTransformationProfileMessageTemplate, systemSettingTransformationProfileName));

                    string sourceFilePath = Path.Combine(packageFolder, SystemSettingsFolder, SystemSettingsFileName);

                    string transformationFilename = string.Format(SystemSettingsTransformationFilenameTemplate, systemSettingTransformationProfileName);
                    string transformationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, transformationFilename);

                    string destinationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, TransformedSystemSettingsFileName);

                    if (this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath))
                    {
                        this.userInterface.WriteLine(Resources.PackageInstaller.SystemSettingTransformationSucceededMessage);
                    }
                    else
                    {
                        this.filesystemAccessor.DeleteFile(destinationFilePath);
                        this.userInterface.WriteLine(Resources.PackageInstaller.SystemSettingTransformationFailedMessage);
                    }
                }

                // execute installation script
                this.userInterface.WriteLine(Resources.PackageInstaller.StartingInstallationPowerShellScriptExecutionMessageTemplate);

                string scriptParameter = string.Format("-{0} {1}", InstallPowerShellScriptDeploymentTypeParameterName, deploymentType);
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.ExecutingInstallScriptMessageTemplate, installScriptPath, scriptParameter));
                this.ExecuteScriptInNewPowerShellHost(installScriptPath, scriptParameter);

                // update package configuration
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.AddingPackageToConfigurationMessageTemplate, package.Id, package.Id));
                packageConfigurationAccessor.AddOrUpdate(new PackageInfo { Id = package.Id, Version = package.Version.ToString() });
            };

            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.StartingInstallationMessageTemplate, package.Id, package.Version));
            packageManager.InstallPackage(package, false, true);
            this.userInterface.WriteLine(Resources.PackageInstaller.InstallationFinishedMessage);

            return true;
        }

        public bool Uninstall(string packageId, SemanticVersion version = null)
        {
            // check if package is installed
            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageIsNotInstalledMessageTemplate, packageId));
                return false;
            }

            // remove the package
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
            this.ExecuteScriptInNewPowerShellHost(uninstallScriptPath);

            // update package configuration
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.RemovingPackageFromConfigurationMessageTemplate, installedPackage.Id, installedPackage.Id));
            this.packageConfigurationAccessor.Remove(installedPackage.Id);

            // remove package files
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.DeletingPackageFolderMessageTemplate, installedPackage.Folder));
            this.filesystemAccessor.DeleteFolder(installedPackage.Folder);

            return true;
        }

        private void ExecuteScriptInNewPowerShellHost(string scriptPath, params string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                throw new ArgumentException("scriptPath");
            }

            if (this.filesystemAccessor.FileExists(scriptPath) == false)
            {
                throw new FileNotFoundException(Resources.Exceptions.PowerShellScriptNotFound, scriptPath);
            }

            using (var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost, this.filesystemAccessor))
            {
                powerShellScriptExecutor.ExecuteScript(scriptPath, parameters);
            }
        }
    }
}