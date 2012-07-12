namespace NuDeploy.Core.Services.Cleanup
{
    public interface ICleanupService
    {
        void Cleanup();

        void Cleanup(string packageId);
    }
}
