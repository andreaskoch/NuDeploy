using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;

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
                    Folder = this.GetPackageInstallationPath(installedPackageInfo)
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