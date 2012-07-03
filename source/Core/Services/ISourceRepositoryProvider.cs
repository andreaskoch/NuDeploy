using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface ISourceRepositoryProvider
    {
        IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations();

        void SaveRepositoryConfiguration(SourceRepositoryConfiguration sourceRepositoryConfiguration);

        void DeleteRepositoryConfiguration(string repositoryName);
    }
}
