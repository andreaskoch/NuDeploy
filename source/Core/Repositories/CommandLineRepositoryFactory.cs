using NuGet;

namespace NuDeploy.Core.Repositories
{
    public class CommandLineRepositoryFactory : PackageRepositoryFactory
    {
        public static readonly string UserAgent = "NuDeploy Command Line";

        public override IPackageRepository CreateRepository(string packageSource)
        {
            var repository = base.CreateRepository(packageSource);
            var httpClientEvents = repository as IHttpClientEvents;

            if (httpClientEvents != null)
            {
                httpClientEvents.SendingRequest += (sender, args) =>
                    {
                        string userAgent = HttpUtility.CreateUserAgentString(UserAgent);
                        HttpUtility.SetUserAgent(args.Request, userAgent);
                    };
            }

            return repository;
        }
    }
}