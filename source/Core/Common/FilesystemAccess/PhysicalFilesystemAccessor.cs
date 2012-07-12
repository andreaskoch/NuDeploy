using System;
using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.Logging;

namespace NuDeploy.Core.Common.FilesystemAccess
{
    public class PhysicalFilesystemAccessor : IFilesystemAccessor
    {
        private readonly IActionLogger logger;

        private readonly IEncodingProvider encodingProvider;

        public PhysicalFilesystemAccessor(IActionLogger logger, IEncodingProvider encodingProvider)
        {
            this.logger = logger;
            this.encodingProvider = encodingProvider;
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            return File.Exists(filePath);
        }

        public bool DirectoryExists(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return false;
            }

            return Directory.Exists(directoryPath);
        }

        public bool MoveFile(string sourceFilePath, string targetFilePath)
        {
            if (string.IsNullOrWhiteSpace(targetFilePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.MoveFilePathIsNullOrEmptyMessage);
                return false;
            }

            if (!this.FileExists(sourceFilePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.MoveFileSourceFileDoesNotExistMessageTemplate, sourceFilePath, targetFilePath);
                return false;
            }

            if (this.FileExists(targetFilePath))
            {
                this.DeleteFile(targetFilePath);
            }

            try
            {
                File.Move(sourceFilePath, targetFilePath);
                return true;
            }
            catch (Exception exception)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.MoveFileExceptionMessageTemplate, sourceFilePath, targetFilePath, exception);
            }

            return false;
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFilePathIsNullOrEmptyMessage);
                return false;
            }

            if (!this.FileExists(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFileDoesNotExistMessageTemplate, filePath);
                return false;
            }

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFileIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception exception)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFileExceptionMessageTemplate, filePath, exception);
            }
            
            return false;
        }

        public bool DeleteFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFolderPathIsNullOrEmptyMessage);
                return false;
            }

            if (!this.DirectoryExists(folderPath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFolderDoesNotExistMessageTemplate, folderPath);
                return false;
            }

            try
            {
                Directory.Delete(folderPath, true);
                return true;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFolderIOExceptionMessageTemplate, folderPath, fileAccessException);
            }
            catch (Exception exception)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.DeleteFolderExceptionMessageTemplate, folderPath, exception);
            }

            return false;
        }

        public string GetFileContent(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetFileContentPathIsNullOrEmptyMessage);
                return null;
            }

            if (!this.FileExists(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetFileContentDoesNotExistMessageTemplate, filePath);
                return null;
            }

            try
            {
                return File.ReadAllText(filePath, this.encodingProvider.GetEncoding());
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetFileContentIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetFileContentExceptionMessageTemplate, filePath, generalException);
            }

            return null;
        }

        public TextReader GetTextReader(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextReaderPathIsNullOrEmptyMessage);
                return null;
            }

            if (!this.FileExists(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextReaderDoesNotExistMessageTemplate);
                return null;
            }

            try
            {
                TextReader textReader = new StreamReader(File.OpenRead(filePath), this.encodingProvider.GetEncoding());
                return textReader;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextReaderIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextReaderExceptionMessageTemplate, filePath, generalException);
            }

            return null;
        }

        public TextWriter GetTextWriter(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextWriterPathIsNullOrEmpty);
                return null;
            }

            try
            {
                TextWriter textWriter = new StreamWriter(File.OpenWrite(filePath), this.encodingProvider.GetEncoding());
                return textWriter;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextWriterIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetTextWriterExceptionMessageTemplate, filePath, generalException);
            }

            return null;
        }

        public Stream GetNewFileStream(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetNewFileStreamPathIsNullOrEmpty);
                return null;
            }

            try
            {
                return File.Create(filePath);
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetNewFileStreamIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.GetNewFileStreamExceptionMessageTemplate, filePath, generalException);
            }

            return null;
        }

        public bool WriteContentToFile(string content, string filePath)
        {
            if (content == null)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.WriteContentToFileContentIsNullMessageTemplate, filePath);
                return false;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.WriteContentToFilePathIsNullOrEmptyMessage);
                return false;
            }

            try
            {
                File.WriteAllText(filePath, content, this.encodingProvider.GetEncoding());
                return true;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.WriteContentToFileIOExceptionMessageTemplate, filePath, fileAccessException);
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.WriteContentToFileExceptionMessageTemplate, filePath, generalException);
            }

            return false;
        }

        public bool CreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.CreateDirectoryPathIsNullOrEmptyMessage);
                return false;
            }

            if (this.DirectoryExists(path))
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.CreateDirectoryPathAlreadyExistsMessageTemplate, path);
                return false;
            }

            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception generalException)
            {
                this.logger.Log(Resources.PhysicalFilesystemAccessor.CreateDirectoryExceptionMessageTemplate, path, generalException);
            }

            return false;
        }

        public bool CopyFile(string sourceFilePath, string targetPath)
        {
            if (!this.FileExists(sourceFilePath))
            {
                return false;
            }

            try
            {
                File.Copy(sourceFilePath, targetPath, true);
                return true;
            }
            catch (Exception generalException)
            {
                this.logger.Log("Unable to copy file \"{0}\" to \"{1}\".", sourceFilePath, targetPath, generalException);
            }

            return false;
        }
    }
}