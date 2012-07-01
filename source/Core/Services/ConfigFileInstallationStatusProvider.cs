using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services
{
    public class ConfigFileInstallationStatusProvider : IInstallationStatusProvider
    {
        private const string PackageConfigurationFileName = "packages.config";

        private readonly string packageConfigurationFilePath;

        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageConfigurationFileReader packageConfigurationFileReader;

        public ConfigFileInstallationStatusProvider(ApplicationInformation applicationInformation, IPackageConfigurationFileReader packageConfigurationFileReader)
        {
            this.applicationInformation = applicationInformation;
            this.packageConfigurationFileReader = packageConfigurationFileReader;

            this.packageConfigurationFilePath = this.GetPackageConfigurationFilePath();
        }

        public bool IsInstalled(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            IEnumerable<PackageInfo> installedPackages = this.packageConfigurationFileReader.GetInstalledPackages(this.packageConfigurationFilePath);
            return installedPackages.Any(pair => pair.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<NuDeployPackageInfo> GetAllPackageInCurrentFolder()
        {
            IEnumerable<PackageInfo> installedPackages = this.packageConfigurationFileReader.GetInstalledPackages(this.packageConfigurationFilePath);
            foreach (PackageInfo package in installedPackages)
            {
                var packageDirectories = Directory.GetDirectories(this.applicationInformation.StartupFolder, string.Format("{0}.*", package.Id));
                foreach (string packageDirectory in packageDirectories)
                {
                    var directoryInfo = new DirectoryInfo(packageDirectory);
                    string packageVersionString = directoryInfo.Name.Replace(string.Format("{0}.", package.Id), string.Empty);
                    var packageVersion = new SemanticVersion(packageVersionString);
                    var isInstalled = packageVersion.Equals(package.Version);

                    yield return new NuDeployPackageInfo { Id = package.Id, Version = packageVersion, Folder = packageDirectory, IsInstalled = isInstalled };
                }
            }
        }

        public NuDeployPackageInfo GetPackageInfo(string id)
        {
            IEnumerable<PackageInfo> installedPackages = this.packageConfigurationFileReader.GetInstalledPackages(this.packageConfigurationFilePath);
            PackageInfo installedPackageInfo = installedPackages.First(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (installedPackageInfo == null)
            {
                return null;
            }

            return new NuDeployPackageInfo
                {
                    Id = installedPackageInfo.Id,
                    Version = installedPackageInfo.Version,
                    Folder = this.GetPackageInstallationPath(installedPackageInfo),
                    IsInstalled = true
                };
        }

        private string GetPackageInstallationPath(PackageInfo packageInfo)
        {
            string packageFolderName = string.Format("{0}.{1}", packageInfo.Id, packageInfo.Version);
            return Path.Combine(this.applicationInformation.StartupFolder, packageFolderName);
        }

        private string GetPackageConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.StartupFolder, PackageConfigurationFileName);
        }
    }
}