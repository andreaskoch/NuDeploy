using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IPackageConfigurationAccessor
    {
        IEnumerable<PackageInfo> GetInstalledPackages();

        void AddOrUpdate(PackageInfo packageInfo);

        void Remove(string packageId);
    }
}