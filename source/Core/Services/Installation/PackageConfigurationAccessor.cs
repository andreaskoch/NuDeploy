using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageConfigurationAccessor : IPackageConfigurationAccessor
    {
        public const string PackageConfigurationFileName = "NuDeploy.Packages.config";

        private static readonly Func<PackageInfo, string> PackageSortKeySelector = p => p.Id;

        private readonly string packageConfigurationFilePath;

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PackageConfigurationAccessor(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.packageConfigurationFilePath = this.GetPackageConfigurationFilePath();
        }

        public IEnumerable<PackageInfo> GetInstalledPackages()
        {
            var packages = this.Load();
            return packages.Distinct().OrderBy(PackageSortKeySelector).ToList();
        }

        public void AddOrUpdate(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                return;
            }

            if (!packageInfo.IsValid)
            {
                return;
            }

            var packages = this.Load().ToDictionary(p => p.Id, p => p);
            if (packages.Keys.Any(id => id.Equals(packageInfo.Id, StringComparison.OrdinalIgnoreCase)))
            {
                var existingEntry = packages[packageInfo.Id];

                // update
                if (existingEntry.Equals(packageInfo) == false)
                {
                    packages[packageInfo.Id] = packageInfo;
                }
            }
            else
            {
                // add
                packages.Add(packageInfo.Id, packageInfo);
            }

            // sort
            var packagesSorted = packages.Values.OrderBy(PackageSortKeySelector).ToArray();

            // save
            this.Save(packagesSorted);
        }

        public void Remove(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return;
            }

            var packages = this.Load().Where(p => p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            this.Save(packages);
        }

        private IEnumerable<PackageInfo> Load()
        {
            if (!this.filesystemAccessor.FileExists(this.packageConfigurationFilePath))
            {
                return new List<PackageInfo>();
            }

            string json = this.filesystemAccessor.GetFileContent(this.packageConfigurationFilePath);
            var packages = JsonConvert.DeserializeObject<PackageInfo[]>(json);

            return packages.Where(p => p.IsValid);
        }

        private void Save(IEnumerable<PackageInfo> packageInfos)
        {
            var validPackages = packageInfos.Where(p => p.IsValid).ToArray();
            string json = JsonConvert.SerializeObject(validPackages);
            this.filesystemAccessor.WriteContentToFile(json, this.packageConfigurationFilePath);
        }

        private string GetPackageConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, PackageConfigurationFileName);
        }
    }
}