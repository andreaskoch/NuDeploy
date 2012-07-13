using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class PackageRepositoryBrowser : IPackageRepositoryBrowser
    {
        private readonly IPackageRepository[] repositories;

        private readonly SourceRepositoryConfiguration[] repositoryConfigurationConfigurations;

        public PackageRepositoryBrowser(ISourceRepositoryProvider sourceRepositoryProvider, IPackageRepositoryFactory packageRepositoryFactory)
        {
            if (sourceRepositoryProvider == null)
            {
                throw new ArgumentNullException("sourceRepositoryProvider");
            }

            if (packageRepositoryFactory == null)
            {
                throw new ArgumentNullException("packageRepositoryFactory");
            }

            this.repositoryConfigurationConfigurations = sourceRepositoryProvider.GetRepositoryConfigurations().ToArray();
            this.repositories = this.repositoryConfigurationConfigurations.Select(r => packageRepositoryFactory.CreateRepository(r.Url.ToString())).ToArray();
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