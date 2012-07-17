using System.Collections.Generic;
using System.IO;

namespace NuDeploy.Core.Common.FilesystemAccess
{
    public interface IFilesystemAccessor
    {
        #region file access

        bool FileExists(string filePath);

        bool DirectoryExists(string directoryPath);

        bool MoveFile(string sourceFilePath, string targetFilePath);

        bool DeleteFile(string filePath);

        string GetFileContent(string filePath);

        TextReader GetTextReader(string filePath);

        TextWriter GetTextWriter(string filePath);

        Stream GetWriteStream(string filePath);

        Stream GetReadStream(string filePath);

        bool WriteContentToFile(string content, string filePath);

        bool CopyFile(string sourceFilePath, string targetPath);

        #endregion

        #region directory access

        IEnumerable<FileInfo> GetFiles(string path);

        IEnumerable<DirectoryInfo> GetSubDirectories(string path);

        bool DeleteDirectory(string folderPath);

        bool CreateDirectory(string path);

        bool EnsureParentDirectoryExists(string filePath);

        #endregion
    }
}
