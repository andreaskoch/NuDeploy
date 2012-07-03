using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public class PackageConfigurationAccessor : IPackageConfigurationAccessor
    {
        public const string PackageConfigurationFileName = "packages.config";

        private static readonly Encoding ConfigFileEncoding = Encoding.UTF8;

        private readonly string packageConfigurationFilePath;

        private readonly ApplicationInformation applicationInformation;

        public PackageConfigurationAccessor(ApplicationInformation applicationInformation)
        {
            this.applicationInformation = applicationInformation;
            this.packageConfigurationFilePath = this.GetPackageConfigurationFilePath();
        }

        public IEnumerable<PackageInfo> GetInstalledPackages()
        {
            if (!File.Exists(this.packageConfigurationFilePath))
            {
                return new List<PackageInfo>();
            }

            string json = File.ReadAllText(this.packageConfigurationFilePath, ConfigFileEncoding);
            var packages = JsonConvert.DeserializeObject<PackageInfo[]>(json);

            return packages.Distinct().OrderBy(p => p.Id).ToList();
        }

        public void AddOrUpdate(PackageInfo packageInfo)
        {
            throw new NotImplementedException();
        }

        public void Remove(string packageId)
        {
            throw new NotImplementedException();
        }

        private string GetPackageConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.StartupFolder, PackageConfigurationFileName);
        }
    }
}