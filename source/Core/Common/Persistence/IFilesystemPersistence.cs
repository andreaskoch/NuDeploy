namespace NuDeploy.Core.Common.Persistence
{
    public interface IFilesystemPersistence<T>
    {
        bool Save(T objectsToPersist, string storageLocation);

        T Load(string storageLocation);
    }
}