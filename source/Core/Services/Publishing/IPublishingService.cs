namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishingService
    {
        IServiceResult PublishPackage(string packagePath, string packageServerConfigurationName);
    }
}