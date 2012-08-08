namespace NuDeploy.Core.Services.Installation
{
    public interface IPackageInstaller
    {
        IServiceResult Install(string packageId, DeploymentType deploymentType, bool forceInstallation, string[] packageConfigurationProfiles, string[] buildConfigurationProfiles);
    }
}