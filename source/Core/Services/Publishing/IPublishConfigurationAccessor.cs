using System.Collections.Generic;

namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishConfigurationAccessor
    {
        PublishConfiguration GetPublishConfiguration(string configurationName);

        IEnumerable<PublishConfiguration> GetPublishConfigurations();

        bool AddOrUpdatePublishConfiguration(string configurationName, string publishLocation, string apiKey);

        bool DeletePublishConfiguration(string configurationName);

        bool ResetPublishConfiguration();
    }
}