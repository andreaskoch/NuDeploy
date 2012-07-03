using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services
{
    public class PackageRepositoryBrowser : IPackageRepositoryBrowser
    {
        private readonly ISourceRepositoryProvider sourceRepositoryProvider;

        private readonly IPackageRepositoryFactory packageRepositoryFactory;

        private readonly IPackageRepository[] repositories;

        private readonly SourceRepository[] repositoryConfigurations;

        public PackageRepositoryBrowser(ISourceRepositoryProvider sourceRepositoryProvider, IPackageRepositoryFactory packageRepositoryFactory)
        {
            this.sourceRepositoryProvider = sourceRepositoryProvider;
            this.packageRepositoryFactory = packageRepositoryFactory;

            this.repositoryConfigurations = this.sourceRepositoryProvider.GetRepositories().ToArray();
            this.repositories = this.repositoryConfigurations.Select(r => this.packageRepositoryFactory.CreateRepository(r.Url)).ToArray();
        }

        public IEnumerable<SourceRepository> RepositoryConfigurations
        {
            get
            {
                return this.repositoryConfigurations;
            }
        }

        public IPackage FindPackage(string packageId, out IPackageRepository packageRepository)
        {
            foreach (var repository in this.repositories)
            {
                var package = repository.FindPackage(packageId);
                if (package != null)
                {
                    packageRepository = repository;
                    return package;
                }
            }

            packageRepository = null;
            return null;
        }
    }
}