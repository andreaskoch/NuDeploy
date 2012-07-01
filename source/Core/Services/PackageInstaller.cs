using System;
using System.IO;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NuGet;

namespace NuDeploy.Core.Services
{
    public class PackageInstaller : IPackageInstaller
    {
        private const string InstallPowerShellScriptName = "Deploy.ps1";

        private const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly string[] installScriptParameters = new[] { "-DeploymentType Full" };

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageRepository packageRepository;

        private readonly PSHost powerShellHost;

        public PackageInstaller(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageRepository packageRepository, PSHost powerShellHost)
        {
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageRepository = packageRepository;
            this.powerShellHost = powerShellHost;
        }

        public bool Install(string packageId, bool forceInstallation)
        {
            // fetch package
            IPackage package = this.packageRepository.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageNotFoundMessageTemplate, packageId, this.packageRepository.Source));
                return false;
            }

            // check if package is already installed
            if (this.installationStatusProvider.IsInstalled(package.Id))
            {
                NuDeployPackageInfo packageInfoOfInstalledVersion = this.installationStatusProvider.GetPackageInfo(package.Id);

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
                                packageInfoOfInstalledVersion.Version));

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
                        this.userInterface.WriteLine(Resources.PackageInstaller.PackageRemovalFailedForceHintMessage);
                        return false;
                    }
                }
            }

            var packageManager = new PackageManager(this.packageRepository, Directory.GetCurrentDirectory());
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

                if (File.Exists(installScriptPath) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PackageInstaller.InstallScriptNotFoundMessageTemplate, installScriptPath, package.Id, package.Version, packageFolder));

                    return;
                }

                this.userInterface.WriteLine(Resources.PackageInstaller.StartingInstallationPowerShellScriptExecutionMessageTemplate);
                this.ExecuteScriptInNewPowerShellHost(installScriptPath, this.installScriptParameters);
            };

            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.StartingInstallationMessageTemplate, package.Id, package.Version));
            packageManager.InstallPackage(package, false, true);
            this.userInterface.WriteLine(Resources.PackageInstaller.InstallationFinishedMessage);

            return true;
        }

        public bool Uninstall(string packageId, SemanticVersion version = null)
        {
            // check if package is installed
            if (!this.installationStatusProvider.IsInstalled(packageId))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageIsNotInstalledMessageTemplate, packageId));
                return false;
            }

            // remove the package
            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId);

            string uninstallScriptPath = Path.Combine(installedPackage.Folder, UninstallPowerShellScriptName);
            if (File.Exists(uninstallScriptPath) == false)
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
            this.userInterface.WriteLine(
                string.Format(Resources.PackageInstaller.StartingUninstallMessageTemplate, installedPackage.Id, installedPackage.Version));

            this.ExecuteScriptInNewPowerShellHost(uninstallScriptPath);

            // remove package files
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.DeletingPackageFolderMessageTemplate, installedPackage.Folder));
            Directory.Delete(installedPackage.Folder, true);

            return true;
        }

        private void ExecuteScriptInNewPowerShellHost(string scriptPath, params string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                throw new ArgumentException("scriptPath");
            }

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("Could not find PowerShell script.", scriptPath);
            }

            using (var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost))
            {
                powerShellScriptExecutor.ExecuteScript(scriptPath, parameters);
            }            
        }
    }
}