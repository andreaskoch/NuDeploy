namespace NuDeploy.Core.Services
{
    public interface ICleanupService
    {
        void Cleanup();

        void Cleanup(string packageId);
    }
}
