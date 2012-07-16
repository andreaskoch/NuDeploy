using System;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class PackageRepositoryFactory : IPackageRepositoryFactory
    {
        private readonly Func<Uri, IHttpClient> httpClientFactory;

        public PackageRepositoryFactory(Func<Uri, IHttpClient> httpClientFactory)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException("httpClientFactory");
            }

            this.httpClientFactory = httpClientFactory;
        }

        public Func<Uri, IHttpClient> HttpClientFactory
        {
            get
            {
                return this.httpClientFactory;
            }
        }

        public virtual IPackageRepository CreateRepository(string packageSource)
        {
            if (string.IsNullOrWhiteSpace(packageSource))
            {
                throw new ArgumentException("packageSource");
            }

            var uri = new Uri(packageSource);
            if (uri.IsFile)
            {
                return new LocalPackageRepository(uri.LocalPath);
            }

            var client = this.HttpClientFactory(uri);
            return new DataServicePackageRepository(client);
        }
    }
}