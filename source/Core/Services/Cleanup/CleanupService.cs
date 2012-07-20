using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Status;

namespace NuDeploy.Core.Services.Cleanup
{
    public class CleanupService : ICleanupService
    {
        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IFilesystemAccessor filesystemAccessor;

        public CleanupService(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IFilesystemAccessor filesystemAccessor)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (installationStatusProvider == null)
            {
                throw new ArgumentNullException("installationStatusProvider");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.filesystemAccessor = filesystemAccessor;
        }

        public bool Cleanup()
        {
            var packages = this.installationStatusProvider.GetPackageInfo().Where(p => p.IsInstalled == false).ToList();

            if (packages.Count == 0)
            {
                this.userInterface.WriteLine(Resources.CleanupService.NoApplicablePackagesForCleanupFound);
                return false;
            }

            this.DeletePackages(packages);
            return true;
        }

        public bool Cleanup(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            var packages =
                this.installationStatusProvider.GetPackageInfo().Where(
                    p => p.IsInstalled == false && p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)).ToList();

            if (packages.Count == 0)
            {
                this.userInterface.WriteLine(string.Format(Resources.CleanupService.NoApplicableVersionForPackageCleanupFoundTemplate, packageId));
                return false;
            }

            this.DeletePackages(packages);
            return true;
        }

        private void DeletePackages(IEnumerable<NuDeployPackageInfo> packages)
        {
            foreach (NuDeployPackageInfo package in packages)
            {
                this.userInterface.WriteLine(string.Format(Resources.CleanupService.DeleteMessageTemplate, package.Id, package.Version));
                this.filesystemAccessor.DeleteDirectory(package.Folder);
            }
        }
    }
}