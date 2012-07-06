using NuGet;

namespace NuDeploy.Core.Services
{
    public interface IPackageInstaller
    {
        bool Install(string packageId, string deploymentType, bool forceInstallation, string systemSettingTransformationProfileName);

        bool Uninstall(string packageId, SemanticVersion version);
    }
}