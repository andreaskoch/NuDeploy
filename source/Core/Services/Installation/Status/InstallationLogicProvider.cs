using System;
using System.Linq;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Status
{
    public class InstallationLogicProvider : IInstallationLogicProvider
    {
        private readonly IInstallationStatusProvider installationStatusProvider;

        public InstallationLogicProvider(IInstallationStatusProvider installationStatusProvider)
        {
            if (installationStatusProvider == null)
            {
                throw new ArgumentNullException("installationStatusProvider");
            }

            this.installationStatusProvider = installationStatusProvider;
        }

        public IServiceResult IsInstallRequired(string packageId, SemanticVersion newPackageVersion, bool forceInstallation)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            if (newPackageVersion == null)
            {
                throw new ArgumentNullException("newPackageVersion");
            }

            if (forceInstallation)
            {
                return new SuccessResult(
                    Resources.InstallationLogicProvider.InstallIsRequiredForceSwitchIsSetMessageTemplate, packageId, newPackageVersion);
            }

            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                return new SuccessResult(
                    Resources.InstallationLogicProvider.InstallationIsRequiredPackageIsNotInstalledMessageTemplate, packageId, newPackageVersion);
            }

            bool newVersionIsGreatedThanTheInstalledVersion = newPackageVersion > installedPackage.Version;
            if (newVersionIsGreatedThanTheInstalledVersion)
            {
                return new SuccessResult(
                    Resources.InstallationLogicProvider.InstallationIsRequiredMessageTemplate,
                    packageId,
                    newPackageVersion,
                    installedPackage.Version);
            }

            return new FailureResult(
                Resources.InstallationLogicProvider.InstallationIsNotRequiredMessageTemplate, packageId, newPackageVersion, installedPackage.Version);
        }

        public IServiceResult IsUninstallRequired(string packageId, SemanticVersion newPackageVersion, DeploymentType deploymentType, bool forceInstallation)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            if (newPackageVersion == null)
            {
                throw new ArgumentNullException("newPackageVersion");
            }

            if (deploymentType == DeploymentType.Update)
            {
                return new FailureResult(Resources.InstallationLogicProvider.UninstallIsNotRequiredDeploymentTypeIsUpdateMessageTemplate, packageId);
            }

            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                return new FailureResult(Resources.InstallationLogicProvider.UninstallIsNotRequiredPackageIsNotInstalledMessageTemplate, packageId);
            }

            if (forceInstallation)
            {
                return new SuccessResult(Resources.InstallationLogicProvider.UninstallIsRequiredForceSwitchIsSetMessageTemplate, packageId);
            }

            bool newVersionIsGreatedThanTheInstalledVersion = newPackageVersion > installedPackage.Version;
            if (newVersionIsGreatedThanTheInstalledVersion)
            {
                return new SuccessResult(
                    Resources.InstallationLogicProvider.UninstallIsRequiredNewPackageVersionIsGreaterThanCurrentVersionMessageTemplate,
                    packageId,
                    newPackageVersion,
                    installedPackage.Version);
            }

            return new FailureResult(
                Resources.InstallationLogicProvider.UninstallIsNotRequiredMessageTemplate, packageId, installedPackage.Version, newPackageVersion);
        }
    }
}