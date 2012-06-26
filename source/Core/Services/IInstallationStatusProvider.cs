using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IInstallationStatusProvider
    {
        bool IsInstalled(string id);

        NuDeployPackageInfo GetPackageInfo(string id);
    }
}