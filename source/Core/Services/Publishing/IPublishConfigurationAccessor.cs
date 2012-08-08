using System.Collections.Generic;

namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishConfigurationAccessor
    {
        PublishConfiguration GetPublishConfiguration(string configurationName);

        IEnumerable<PublishConfiguration> GetPublishConfigurations();

        IServiceResult AddOrUpdatePublishConfiguration(string configurationName, string publishLocation, string apiKey);

        IServiceResult DeletePublishConfiguration(string configurationName);

        IServiceResult ResetPublishConfiguration();
    }
}