using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Configuration;
using NuDeploy.Core.Services.Installation;

using NuGet;

namespace NuDeploy.Core.Services.Status
{
    public class InstallationStatusProvider : IInstallationStatusProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageConfigurationAccessor packageConfigurationAccessor;

        public InstallationStatusProvider(ApplicationInformation applicationInformation, IPackageConfigurationAccessor packageConfigurationAccessor)
        {
            this.applicationInformation = applicationInformation;
            this.packageConfigurationAccessor = packageConfigurationAccessor;
        }

        public IEnumerable<NuDeployPackageInfo> GetPackageInfo()
        {
            return this.GetAllPackages();
        }

        public IEnumerable<NuDeployPackageInfo> GetPackageInfo(string id)
        {
            IEnumerable<NuDeployPackageInfo> packages = this.GetAllPackages();
            return packages.Where(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<NuDeployPackageInfo> GetAllPackages()
        {
            IEnumerable<PackageInfo> installedPackages = this.packageConfigurationAccessor.GetInstalledPackages();
            foreach (PackageInfo package in installedPackages)
            {
                var packageDirectories = Directory.GetDirectories(this.applicationInformation.StartupFolder, string.Format("{0}.*", package.Id));
                foreach (string packageDirectory in packageDirectories)
                {
                    var directoryInfo = new DirectoryInfo(packageDirectory);
                    string packageVersionString = directoryInfo.Name.Replace(string.Format("{0}.", package.Id), string.Empty);
                    var packageVersion = new SemanticVersion(packageVersionString);
                    var isInstalled = packageVersion.ToString().Equals(package.Version);

                    yield return new NuDeployPackageInfo { Id = package.Id, Version = packageVersion, Folder = packageDirectory, IsInstalled = isInstalled };
                }
            }
        }
    }
}