using System;

namespace NuDeploy.Core.Services.Installation
{
    public class DeploymentTypeParser : IDeploymentTypeParser
    {
        public const DeploymentType DefaultDeploymentType = DeploymentType.Full;

        public DeploymentType GetDeploymentType(string deploymentType)
        {
            if (string.IsNullOrWhiteSpace(deploymentType))
            {
                return DefaultDeploymentType;
            }

            DeploymentType recognizedType;
            if (Enum.TryParse(deploymentType.Trim(), true, out recognizedType))
            {
                return recognizedType;
            }

            return DefaultDeploymentType;
        }
    }
}