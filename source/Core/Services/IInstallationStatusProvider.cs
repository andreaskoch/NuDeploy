using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IInstallationStatusProvider
    {
        bool IsInstalled(string id);

        IEnumerable<NuDeployPackageInfo> GetAllPackageInCurrentFolder();

        NuDeployPackageInfo GetPackageInfo(string id);
    }
}