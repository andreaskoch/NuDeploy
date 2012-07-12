namespace NuDeploy.Core.Services.Filesystem
{
    public interface IRelativeFilePathInfoFactory
    {
        RelativeFilePathInfo GetRelativeFilePathInfo(string absoluteFilePath, string basePath);
    }
}