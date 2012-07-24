using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public interface ISourceRepositoryConfigurationFactory
    {
        SourceRepositoryConfiguration GetSourceRepositoryConfiguration(string repositoryName, string repositoryUrl);
    }
}