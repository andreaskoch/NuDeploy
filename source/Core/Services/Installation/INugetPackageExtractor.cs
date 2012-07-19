using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public interface INugetPackageExtractor
    {
        NuDeployPackageInfo Extract(IPackage package, string targetFolder);
    }
}