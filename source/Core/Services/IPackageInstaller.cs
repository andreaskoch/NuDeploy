using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services
{
    public interface IPackageInstaller
    {
        bool Install(IPackage package);

        bool Uninstall(NuDeployPackageInfo installedPackage);
    }
}