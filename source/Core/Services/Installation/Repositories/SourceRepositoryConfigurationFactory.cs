using System;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class SourceRepositoryConfigurationFactory : ISourceRepositoryConfigurationFactory
    {
        public SourceRepositoryConfiguration GetSourceRepositoryConfiguration(string repositoryName, string repositoryUrl)
        {
            if (string.IsNullOrWhiteSpace(repositoryName) || string.IsNullOrWhiteSpace(repositoryUrl))
            {
                return null;
            }

            Uri sourceRepositoryUri;
            if (Uri.TryCreate(repositoryUrl, UriKind.Absolute, out sourceRepositoryUri) == false)
            {
                return null;
            }

            return new SourceRepositoryConfiguration { Name = repositoryName, Url = sourceRepositoryUri };
        }
    }
}