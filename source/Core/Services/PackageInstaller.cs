using System;
using System.IO;
using System.Linq;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NuGet;

namespace NuDeploy.Core.Services
{
    public interface IPackageRepositoryBrowser
    {
        IPackage FindPackage(string packageId);
    }

    public class PackageRepositoryBrowser : IPackageRepositoryBrowser
    {
        private readonly IPackageRepository packageRepository;

        private readonly ISourceRepositoryProvider sourceRepositoryProvider;

        public PackageRepositoryBrowser(IPackageRepository packageRepository, ISourceRepositoryProvider sourceRepositoryProvider)
        {
            this.packageRepository = packageRepository;
            this.sourceRepositoryProvider = sourceRepositoryProvider;
        }

        public IPackage FindPackage(string packageId)
        {
            throw new NotImplementedException();
        }
    }

    public class PackageInstaller : IPackageInstaller
    {
        private const string InstallPowerShellScriptName = "Deploy.ps1";

        private const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly string[] installScriptParameters = new[] { "-DeploymentType Full" };

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly PSHost powerShellHost;

        public PackageInstaller(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageRepositoryBrowser packageRepositoryBrowser, PSHost powerShellHost)
        {
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.powerShellHost = powerShellHost;
        }

        public bool Install(string packageId, bool forceInstallation)
        {
            // fetch package
            IPackage package = this.packageRepositoryBrowser.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageNotFoundMessageTemplate, packageId, this.packageRepositoryBrowser.Source));
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

            var packageManager = new PackageManager(this.packageRepositoryBrowser, Directory.GetCurrentDirectory());
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
            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.PackageIsNotInstalledMessageTemplate, packageId));
                return false;
            }

            // remove the package
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