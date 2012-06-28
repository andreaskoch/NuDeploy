using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services
{
    public class PackageConfigurationFileReader : IPackageConfigurationFileReader
    {
        private const string RootNodePackages = "packages";

        private const string PackageNode = "package";

        private const string PackageAttributeId = "id";

        private const string PackageAttributeVersion = "version";

        public IEnumerable<PackageInfo> GetInstalledPackages(string configurationFilePath)
        {
            if (string.IsNullOrWhiteSpace(configurationFilePath))
            {
                throw new ArgumentException("configurationFilePath");
            }

            if (File.Exists(configurationFilePath) == false)
            {
                return new List<PackageInfo>();
            }

            XDocument packagesConfiguration = XDocument.Load(configurationFilePath);
            return
                packagesConfiguration.Descendants(RootNodePackages).Descendants(PackageNode).Where(
                    p => p.Attribute(PackageAttributeId) != null && p.Attribute(PackageAttributeVersion) != null).Select(
                        p =>
                        new PackageInfo
                            { 
                                Id = p.Attribute(PackageAttributeId).Value, 
                                Version = new SemanticVersion(p.Attribute(PackageAttributeVersion).Value)
                            });
        }
    }
}