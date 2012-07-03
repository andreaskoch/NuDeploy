using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public interface ISourceRepositoryProvider
    {
        IEnumerable<SourceRepository> GetRepositories();

        void SaveRepository(SourceRepository sourceRepository);

        void DeleteRepository(string repositoryName);
    }
}
