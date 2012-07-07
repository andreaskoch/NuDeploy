namespace NuDeploy.Core.Common
{
    public interface IFilesystemAccessor
    {
        bool FileExists(string filePath);

        bool DirectoryExists(string directoryPath);
    }
}
