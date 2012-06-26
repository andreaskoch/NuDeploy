using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IPackageConfigurationFileReader
    {
        IEnumerable<PackageInfo> GetInstalledPackages(string configurationFilePath);
    }
}