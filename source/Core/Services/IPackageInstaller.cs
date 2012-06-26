using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface IPackageInstaller
    {
        bool Uninstall(NuDeployPackageInfo installedPackage);
    }
}