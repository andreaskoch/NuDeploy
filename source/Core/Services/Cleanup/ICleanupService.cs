namespace NuDeploy.Core.Services.Cleanup
{
    public interface ICleanupService
    {
        bool Cleanup();

        bool Cleanup(string packageId);
    }
}
