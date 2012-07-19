using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageUninstaller
    {
        bool Uninstall(string packageId, SemanticVersion version);
    }
}