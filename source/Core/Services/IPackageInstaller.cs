using NuGet;

namespace NuDeploy.Core.Services
{
    public interface IPackageInstaller
    {
        bool Install(string packageId, bool forceInstallation);

        bool Uninstall(string packageId, SemanticVersion version);
    }
}