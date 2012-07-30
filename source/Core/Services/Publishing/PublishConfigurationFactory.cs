namespace NuDeploy.Core.Services.Publishing
{
    public class PublishConfigurationFactory : IPublishConfigurationFactory
    {
        public PublishConfiguration GetPublishConfiguration(string configurationName, string publishLocation, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(publishLocation))
            {
                return null;
            }

            return new PublishConfiguration { Name = configurationName, PublishLocation = publishLocation, ApiKey = apiKey };
        }
    }
}