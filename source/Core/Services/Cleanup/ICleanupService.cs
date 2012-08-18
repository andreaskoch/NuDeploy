namespace NuDeploy.Core.Services.Cleanup
{
    public interface ICleanupService
    {
        IServiceResult Cleanup();

        IServiceResult Cleanup(string packageId);
    }
}
