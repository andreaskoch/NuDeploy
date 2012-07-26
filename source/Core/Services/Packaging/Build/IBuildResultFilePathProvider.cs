using NuDeploy.Core.Services.Filesystem;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public interface IBuildResultFilePathProvider
    {
        RelativeFilePathInfo[] GetWebsiteFilePaths();

        RelativeFilePathInfo[] GetWebApplicationFilePaths();

        RelativeFilePathInfo[] GetApplicationFilePaths();

        RelativeFilePathInfo[] GetDeploymentPackageAdditionFilePaths();
    }
}