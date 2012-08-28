using NuDeploy.Core.Services.Filesystem;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public interface IBuildResultFilePathProvider
    {
        RelativeFilePathInfo[] GetWebsiteFilePaths(string buildFolder);

        RelativeFilePathInfo[] GetWebApplicationFilePaths(string buildFolder);

        RelativeFilePathInfo[] GetApplicationFilePaths(string buildFolder);

        RelativeFilePathInfo[] GetDeploymentPackageAdditionFilePaths(string buildFolder);
    }
}