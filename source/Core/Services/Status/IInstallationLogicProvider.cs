using NuDeploy.Core.Services.Installation;

using NuGet;

namespace NuDeploy.Core.Services.Status
{
    public interface IInstallationLogicProvider
    {
        bool IsInstallRequired(string packageId, SemanticVersion newPackageVersion, bool forceInstallation);

        bool IsUninstallRequired(string packageId, SemanticVersion newPackageVersion, DeploymentType deploymentType, bool forceInstallation);        
    }
}