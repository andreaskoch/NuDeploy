using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IInstallationStatusProvider
    {
        IEnumerable<NuDeployPackageInfo> GetPackageInfo();

        IEnumerable<NuDeployPackageInfo> GetPackageInfo(string id);
    }
}