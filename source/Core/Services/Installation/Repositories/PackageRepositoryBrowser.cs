using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Status;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class PackageRepositoryBrowser : IPackageRepositoryBrowser
    {
        private readonly ISourceRepositoryProvider sourceRepositoryProvider;

        private readonly IPackageRepositoryFactory packageRepositoryFactory;

        private readonly IPackageRepository[] repositories;

        private readonly SourceRepositoryConfiguration[] repositoryConfigurationConfigurations;

        public PackageRepositoryBrowser(ISourceRepositoryProvider sourceRepositoryProvider, IPackageRepositoryFactory packageRepositoryFactory)
        {
            this.sourceRepositoryProvider = sourceRepositoryProvider;
            this.packageRepositoryFactory = packageRepositoryFactory;

            this.repositoryConfigurationConfigurations = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToArray();
            this.repositories = this.repositoryConfigurationConfigurations.Select(r => this.packageRepositoryFactory.CreateRepository(r.Url.ToString())).ToArray();
        }

        public IEnumerable<SourceRepositoryConfiguration> RepositoryConfigurations
        {
            get
            {
                return this.repositoryConfigurationConfigurations;
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