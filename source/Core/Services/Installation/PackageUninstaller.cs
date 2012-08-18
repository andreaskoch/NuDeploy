using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Status;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageUninstaller : IPackageUninstaller
    {
        public const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IPowerShellExecutor powerShellExecutor;

        public PackageUninstaller(IInstallationStatusProvider installationStatusProvider, IPackageConfigurationAccessor packageConfigurationAccessor, IFilesystemAccessor filesystemAccessor, IPowerShellExecutor powerShellExecutor)
        {
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

            this.installationStatusProvider = installationStatusProvider;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.filesystemAccessor = filesystemAccessor;
            this.powerShellExecutor = powerShellExecutor;
        }

        public IServiceResult Uninstall(string packageId, SemanticVersion version)
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
                return new FailureResult(Resources.PackageInstaller.PackageIsNotInstalledMessageTemplate, packageId);
            }

            // find the uninstall script
            string uninstallScriptPath = Path.Combine(installedPackage.Folder, UninstallPowerShellScriptName);
            if (this.filesystemAccessor.FileExists(uninstallScriptPath) == false)
            {
                return new FailureResult(
                    Resources.PackageInstaller.UninstallScriptNotFoundMessageTemplate,
                    UninstallPowerShellScriptName,
                    installedPackage.Id,
                    installedPackage.Version,
                    installedPackage.Folder);
            }

            // uninstall
            IServiceResult powerShellResult = this.powerShellExecutor.ExecuteScript(uninstallScriptPath);
            if (powerShellResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.PackageInstaller.ExecutingUninstallScriptFailedMessageTemplate, uninstallScriptPath)
                    {
                        InnerResult = powerShellResult
                    };
            }

            // update package configuration
            IServiceResult removePackageFromConfigResult = this.packageConfigurationAccessor.Remove(installedPackage.Id);
            if (removePackageFromConfigResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(
                    Resources.PackageInstaller.UninstallSucceededButPackageCouldNotBeRemovedFromConfigurationMessageTemplate,
                    installedPackage.Id,
                    installedPackage.Version)
                    {
                        InnerResult = removePackageFromConfigResult
                    };
            }

            // remove package files
            if (!this.filesystemAccessor.DeleteDirectory(installedPackage.Folder))
            {
                return new FailureResult(
                    Resources.PackageInstaller.UninstallSucceededButPackageDirectoryCouldNotBeRemovedMessageTemplate,
                    installedPackage.Id,
                    installedPackage.Version,
                    installedPackage.Folder);
            }

            return new SuccessResult(Resources.PackageInstaller.UninstallSucceededMessageTemplate, installedPackage.Id, installedPackage.Version);
        }
    }
}