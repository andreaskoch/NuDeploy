namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishConfigurationFactory
    {
        PublishConfiguration GetPublishConfiguration(string configurationName, string publishLocation, string apiKey);
    }
}