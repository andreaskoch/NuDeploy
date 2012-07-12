namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public interface IDeploymentScriptResourceDownloader
    {
        void Download(string targetFolder);
    }
}