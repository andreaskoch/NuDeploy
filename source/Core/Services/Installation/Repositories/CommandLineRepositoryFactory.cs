using System;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class CommandLineRepositoryFactory : PackageRepositoryFactory
    {
        public static readonly string UserAgent = NuDeployConstants.NuDeployCommandLinePackageId;

        public override IPackageRepository CreateRepository(string packageSource)
        {
            if (string.IsNullOrWhiteSpace(packageSource))
            {
                throw new ArgumentException("packageSource");
            }

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