using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Status;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageUninstaller : IPackageUninstaller
    {
        public const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IPowerShellExecutor powerShellExecutor;

        public PackageUninstaller(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageConfigurationAccessor packageConfigurationAccessor, IFilesystemAccessor filesystemAccessor, IPowerShellExecutor powerShellExecutor)
        {
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

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (powerShellExecutor == null)
            {
                throw new ArgumentNullException("powerShellExecutor");
            }

            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.filesystemAccessor = filesystemAccessor;
            this.powerShellExecutor = powerShellExecutor;
        }

        public bool Uninstall(string packageId, SemanticVersion version)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            // check if package is installed
            NuDeployPackageInfo installedPackage = version == null
                                                       ? this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled)
                                                       : this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(
                                                           p => p.Version.Equals(version));
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
            this.userInterface.WriteLine(
                string.Format(Resources.PackageInstaller.RemovingPackageFromConfigurationMessageTemplate, installedPackage.Id, installedPackage.Id));
            this.packageConfigurationAccessor.Remove(installedPackage.Id);

            // remove package files
            this.userInterface.WriteLine(string.Format(Resources.PackageInstaller.DeletingPackageFolderMessageTemplate, installedPackage.Folder));
            this.filesystemAccessor.DeleteDirectory(installedPackage.Folder);

            return true;
        }
    }
}