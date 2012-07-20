using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Status
{
    public class InstallationStatusProvider : IInstallationStatusProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        private readonly IFilesystemAccessor filesystemAccessor;

        public InstallationStatusProvider(ApplicationInformation applicationInformation, IPackageConfigurationAccessor packageConfigurationAccessor, IFilesystemAccessor filesystemAccessor)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (packageConfigurationAccessor == null)
            {
                throw new ArgumentNullException("packageConfigurationAccessor");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.applicationInformation = applicationInformation;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
            this.filesystemAccessor = filesystemAccessor;
        }

        public IEnumerable<NuDeployPackageInfo> GetPackageInfo()
        {
            return this.GetAllPackages();
        }

        public IEnumerable<NuDeployPackageInfo> GetPackageInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new NuDeployPackageInfo[] { };
            }

            IEnumerable<NuDeployPackageInfo> packages = this.GetAllPackages();
            return packages.Where(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<NuDeployPackageInfo> GetAllPackages()
        {
            IEnumerable<PackageInfo> installedPackages = this.packageConfigurationAccessor.GetInstalledPackages();

            foreach (PackageInfo package in installedPackages)
            {
                string packagePrefix = string.Format("{0}.", package.Id);

                var packageDirectories =
                    this.filesystemAccessor.GetSubDirectories(this.applicationInformation.StartupFolder).Where(
                        dir => dir.Name.StartsWith(packagePrefix, StringComparison.OrdinalIgnoreCase));

                foreach (DirectoryInfo packageDirectory in packageDirectories)
                {
                    string packageVersionString = packageDirectory.Name.Replace(string.Format("{0}.", package.Id), string.Empty);
                    var packageVersion = new SemanticVersion(packageVersionString);
                    var isInstalled = packageVersion.ToString().Equals(package.Version);

                    yield return new NuDeployPackageInfo
                        {
                            Id = package.Id,
                            Version = packageVersion,
                            Folder = packageDirectory.FullName,
                            IsInstalled = isInstalled
                        };
                }
            }
        }
    }
}