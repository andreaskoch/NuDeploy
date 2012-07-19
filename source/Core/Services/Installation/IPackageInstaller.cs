using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageInstaller
    {
        bool Install(string packageId, DeploymentType deploymentType, bool forceInstallation, string[] systemSettingTransformationProfileNames);

        bool Uninstall(string packageId, SemanticVersion version);
    }
}