using System.IO;

namespace NuDeploy.Core.Common
{
    public interface IFilesystemAccessor
    {
        bool FileExists(string filePath);

        bool DirectoryExists(string directoryPath);

        bool MoveFile(string sourceFilePath, string targetFilePath);

        bool DeleteFile(string filePath);

        bool DeleteFolder(string folderPath);

        string GetFileContent(string filePath);

        TextReader GetTextReader(string filePath);

        TextWriter GetTextWriter(string filePath);

        Stream GetNewFileStream(string filePath);

        bool WriteContentToFile(string content, string filePath);
    }
}
