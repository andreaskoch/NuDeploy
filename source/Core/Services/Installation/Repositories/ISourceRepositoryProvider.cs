using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public interface ISourceRepositoryProvider
    {
        IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations();

        bool SaveRepositoryConfiguration(string repositoryName, string repositoryUrl);

        bool DeleteRepositoryConfiguration(string repositoryName);

        bool ResetRepositoryConfiguration();
    }
}
