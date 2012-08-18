using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageUninstaller
    {
        IServiceResult Uninstall(string packageId, SemanticVersion version);
    }
}