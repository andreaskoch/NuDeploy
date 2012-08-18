using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageConfigurationAccessor
    {
        IEnumerable<PackageInfo> GetInstalledPackages();

        IServiceResult AddOrUpdate(PackageInfo packageInfo);

        IServiceResult Remove(string packageId);
    }
}