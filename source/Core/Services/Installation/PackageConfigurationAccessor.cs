using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;

namespace NuDeploy.Core.Services.Installation
{
    public class PackageConfigurationAccessor : IPackageConfigurationAccessor
    {
        public const string PackageConfigurationFileName = "NuDeploy.Packages.config";

        private static readonly Func<PackageInfo, string> PackageSortKeySelector = p => p.Id;

        private readonly string packageConfigurationFilePath;

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemPersistence<PackageInfo[]> packageInfoFilesystemPersistence;

        public PackageConfigurationAccessor(ApplicationInformation applicationInformation, IFilesystemPersistence<PackageInfo[]> packageInfoFilesystemPersistence)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (packageInfoFilesystemPersistence == null)
            {
                throw new ArgumentNullException("packageInfoFilesystemPersistence");
            }

            this.applicationInformation = applicationInformation;
            this.packageInfoFilesystemPersistence = packageInfoFilesystemPersistence;
            this.packageConfigurationFilePath = this.GetPackageConfigurationFilePath();
        }

        public IEnumerable<PackageInfo> GetInstalledPackages()
        {
            var packages = this.GetExistingPackageConfigurationList();
            return packages.Where(p => p.IsValid).Distinct().OrderBy(PackageSortKeySelector).ToList();
        }

        public bool AddOrUpdate(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                throw new ArgumentNullException("packageInfo");
            }

            if (!packageInfo.IsValid)
            {
                return false;
            }

            var packages = this.GetExistingPackageConfigurationList().ToDictionary(p => p.Id, p => p);
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
            return this.SaveNewPackageConfigurationList(packagesSorted);
        }

        public bool Remove(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("packageId");
            }

            var existingPackageList = this.GetExistingPackageConfigurationList().ToList();
            if (!existingPackageList.Any(p => p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var newPackageList = existingPackageList.Where(p => p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            return this.SaveNewPackageConfigurationList(newPackageList);
        }

        private IEnumerable<PackageInfo> GetExistingPackageConfigurationList()
        {
            var packages = this.packageInfoFilesystemPersistence.Load(this.packageConfigurationFilePath) ?? new PackageInfo[] { };
            return packages.Where(p => p.IsValid);
        }

        private bool SaveNewPackageConfigurationList(IEnumerable<PackageInfo> packageInfos)
        {
            var validPackages = packageInfos.Where(p => p.IsValid).ToArray();
            return this.packageInfoFilesystemPersistence.Save(validPackages, this.packageConfigurationFilePath);
        }

        private string GetPackageConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, PackageConfigurationFileName);
        }
    }
}