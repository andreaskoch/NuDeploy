using System.Collections.Generic;

namespace NuDeploy.Core.Services.Publishing
{
    public interface IPublishConfigurationAccessor
    {
        PublishConfiguration GetPublishConfiguration(string configurationName);

        IEnumerable<PublishConfiguration> GetPublishConfigurations();
    }
}