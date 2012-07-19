using System;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation;

using NuGet;

namespace NuDeploy.Core.Services.Status
{
    public class InstallationLogicProvider : IInstallationLogicProvider
    {
        private readonly IInstallationStatusProvider installationStatusProvider;

        public InstallationLogicProvider(IInstallationStatusProvider installationStatusProvider)
        {
            this.installationStatusProvider = installationStatusProvider;
        }

        public bool IsInstallRequired(string packageId, SemanticVersion newPackageVersion, bool forceInstallation)
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
                return true;
            }

            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                return true;
            }

            return newPackageVersion > installedPackage.Version;
        }

        public bool IsUninstallRequired(string packageId, SemanticVersion newPackageVersion, DeploymentType deploymentType, bool forceInstallation)
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
                return false;
            }

            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId).FirstOrDefault(p => p.IsInstalled);
            if (installedPackage == null)
            {
                return false;
            }

            if (forceInstallation)
            {
                return true;
            }

            return newPackageVersion > installedPackage.Version;
        }
    }
}