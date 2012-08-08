using NuGet;

namespace NuDeploy.Core.Services.Installation.Status
{
    public interface IInstallationLogicProvider
    {
        IServiceResult IsInstallRequired(string packageId, SemanticVersion newPackageVersion, bool forceInstallation);

        IServiceResult IsUninstallRequired(string packageId, SemanticVersion newPackageVersion, DeploymentType deploymentType, bool forceInstallation);        
    }
}