using NuDeploy.Core.Services.Filesystem;

namespace NuDeploy.Core.Services.Packaging.Configuration
{
    public interface IBuildResultFilePathProvider
    {
        RelativeFilePathInfo[] GetWesbiteFilePaths();

        RelativeFilePathInfo[] GetWebApplicationFilePaths();

        RelativeFilePathInfo[] GetApplicationFilePaths();

        RelativeFilePathInfo[] GetDeploymentPackageAdditionFilePaths();

        RelativeFilePathInfo GetNuspecFilePath(string buildConfiguration);
    }
}