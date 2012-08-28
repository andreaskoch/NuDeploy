namespace NuDeploy.Core.Services.Packaging.PrePackaging
{
    public interface IPrepackagingService
    {
        IServiceResult Prepackage(string buildFolder);
    }
}