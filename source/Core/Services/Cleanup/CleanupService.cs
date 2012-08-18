using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Installation.Status;

namespace NuDeploy.Core.Services.Cleanup
{
    public class CleanupService : ICleanupService
    {
        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IFilesystemAccessor filesystemAccessor;

        public CleanupService(IInstallationStatusProvider installationStatusProvider, IFilesystemAccessor filesystemAccessor)
        {
            if (installationStatusProvider == null)
            {
                throw new ArgumentNullException("installationStatusProvider");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.installationStatusProvider = installationStatusProvider;
            this.filesystemAccessor = filesystemAccessor;
        }

        public IServiceResult Cleanup()
        {
            var packages = this.installationStatusProvider.GetPackageInfo().Where(p => p.IsInstalled == false).ToList();

            if (packages.Count == 0)
            {
                return new FailureResult(Resources.CleanupService.NoApplicablePackagesForCleanupFound);
            }

            var deleteResults = this.DeletePackages(packages);
            if (deleteResults.Any(r => r.Status == ServiceResultType.Failure))
            {
                return new FailureResult(
                    Resources.CleanupService.CleanupFailedMessageTemplate, string.Join(Environment.NewLine, deleteResults.Select(result => result.Message)));
            }

            return new SuccessResult(Resources.CleanupService.CleanupSucceeded);
        }

        public IServiceResult Cleanup(string packageId)
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
                return new FailureResult(Resources.CleanupService.NoApplicableVersionForPackageCleanupFoundTemplate, packageId);
            }

            var deleteResults = this.DeletePackages(packages);
            if (deleteResults.Any(r => r.Status == ServiceResultType.Failure))
            {
                return new FailureResult(
                    Resources.CleanupService.PackageSpecificCleanupFailedMessageTemplate,
                    packageId,
                    string.Join(Environment.NewLine, deleteResults.Select(result => result.Message)));
            }

            return new SuccessResult(Resources.CleanupService.PackageSpecificCleanupSucceededMessageTemplate, packageId);
        }

        private IServiceResult[] DeletePackages(IEnumerable<NuDeployPackageInfo> packages)
        {
            var results = new List<IServiceResult>();

            foreach (NuDeployPackageInfo package in packages)
            {
                var deleteResult = this.filesystemAccessor.DeleteDirectory(package.Folder);
                if (deleteResult)
                {
                    results.Add(new SuccessResult(Resources.CleanupService.DeleteSucceededMessageTemplate, package.Id, package.Version));
                }
                else
                {
                    results.Add(new FailureResult(Resources.CleanupService.DeleteFailedMessageTemplate, package.Id, package.Version));
                }
            }

            return results.ToArray();
        }
    }
}