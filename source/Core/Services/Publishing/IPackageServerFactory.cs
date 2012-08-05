using NuGet;

namespace NuDeploy.Core.Services.Publishing
{
    public interface IPackageServerFactory
    {
        PackageServer GetPackageServer(string serverLocation);
    }
}