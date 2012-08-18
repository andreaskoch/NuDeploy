using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public interface ISourceRepositoryProvider
    {
        IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations();

        IServiceResult SaveRepositoryConfiguration(string repositoryName, string repositoryUrl);

        IServiceResult DeleteRepositoryConfiguration(string repositoryName);

        IServiceResult ResetRepositoryConfiguration();
    }
}
