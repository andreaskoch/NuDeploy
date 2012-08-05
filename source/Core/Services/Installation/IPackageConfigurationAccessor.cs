using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageConfigurationAccessor
    {
        IEnumerable<PackageInfo> GetInstalledPackages();

        bool AddOrUpdate(PackageInfo packageInfo);

        bool Remove(string packageId);
    }
}