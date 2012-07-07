namespace NuDeploy.Core.Common
{
    public interface IFilesystemAccessor
    {
        bool FileExists(string filePath);

        bool DirectoryExists(string directoryPath);

        bool MoveFile(string sourceFilePath, string targetFilePath);

        bool DeleteFile(string filePath);

        bool DeleteFolder(string folderPath);
    }
}
