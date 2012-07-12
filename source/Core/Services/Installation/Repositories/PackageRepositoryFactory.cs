using System;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class PackageRepositoryFactory : IPackageRepositoryFactory
    {
        private static readonly PackageRepositoryFactory DefaultPackageRepositoryInstance = new PackageRepositoryFactory();

        private static readonly Func<Uri, IHttpClient> DefaultHttpClientFactory = u => new RedirectedHttpClient(u);

        private Func<Uri, IHttpClient> httpClientFactory;

        public static PackageRepositoryFactory Default
        {
            get
            {
                return DefaultPackageRepositoryInstance;
            }
        }

        public Func<Uri, IHttpClient> HttpClientFactory
        {
            get { return this.httpClientFactory ?? DefaultHttpClientFactory; }
            set { this.httpClientFactory = value; }
        }

        public virtual IPackageRepository CreateRepository(string packageSource)
        {
            if (packageSource == null)
            {
                throw new ArgumentNullException("packageSource");
            }

            var uri = new Uri(packageSource);
            if (uri.IsFile)
            {
                return new LocalPackageRepository(uri.LocalPath);
            }

            var client = this.HttpClientFactory(uri);

            // Make sure we get resolve any fwlinks before creating the repository
            return new DataServicePackageRepository(client);
        }
    }
}