using System.Collections.Generic;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public interface IPackageRepositoryBrowser
    {
        IEnumerable<SourceRepositoryConfiguration> RepositoryConfigurations { get; }

        IPackage FindPackage(string packageId);
    }
}