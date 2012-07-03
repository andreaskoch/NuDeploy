using System.Collections.Generic;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services
{
    public interface IPackageRepositoryBrowser
    {
        IEnumerable<SourceRepository> RepositoryConfigurations { get; }

        IPackage FindPackage(string packageId, out IPackageRepository packageRepository);
    }
}