using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public class CleanupService : ICleanupService
    {
        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        public CleanupService(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider)
        {
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
        }

        public void Cleanup()
        {
            var packages = this.installationStatusProvider.GetAllPackageInCurrentFolder().Where(p => p.IsInstalled == false).ToList();

            if (packages.Count == 0)
            {
                this.userInterface.WriteLine(Resources.CleanupService.NoApplicablePackagesForCleanupFound);
                return;
            }

            this.DeletePackages(packages);            
        }

        public void Cleanup(string packageId)
        {
            var packages =
                this.installationStatusProvider.GetAllPackageInCurrentFolder().Where(
                    p => p.IsInstalled == false && p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)).ToList();

            if (packages.Count == 0)
            {
                this.userInterface.WriteLine(string.Format(Resources.CleanupService.NoApplicableVersionForPackageCleanupFoundTemplate, packageId));
                return;
            }

            this.DeletePackages(packages);
        }

        private void DeletePackages(IEnumerable<NuDeployPackageInfo> packages)
        {
            foreach (NuDeployPackageInfo package in packages)
            {
                this.userInterface.WriteLine(string.Format(Resources.CleanupService.DeleteMessageTemplate, package.Id, package.Version));
                Directory.Delete(package.Folder, true);
            }
        }
    }
}