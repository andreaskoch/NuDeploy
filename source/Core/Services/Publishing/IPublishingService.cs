namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishingService
    {
        bool PublishPackage(string packagePath, string packageServerConfigurationName);
    }
}