namespace NuDeploy.Core.Services.Packaging
{
    public interface IBuildOutputPackagingService
    {
        IServiceResult Package(string buildOutputFolderPath);
    }
}