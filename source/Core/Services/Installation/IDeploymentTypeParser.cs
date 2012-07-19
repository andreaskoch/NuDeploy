namespace NuDeploy.Core.Services.Installation
{
    public interface IDeploymentTypeParser
    {
        DeploymentType GetDeploymentType(string deploymentType);
    }
}