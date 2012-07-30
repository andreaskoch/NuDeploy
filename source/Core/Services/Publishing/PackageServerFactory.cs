using System;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Publishing
{
    public class PackageServerFactory : IPackageServerFactory
    {
        public PackageServer GetPackageServer(string serverLocation)
        {
            if (string.IsNullOrWhiteSpace(serverLocation))
            {
                throw new ArgumentException("serverLocation");
            }

            return new PackageServer(serverLocation, NuDeployConstants.NuDeployCommandLinePackageId);
        }
    }
}