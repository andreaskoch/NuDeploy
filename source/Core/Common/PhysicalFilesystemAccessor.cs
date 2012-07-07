using System;
using System.IO;

namespace NuDeploy.Core.Common
{
    public class PhysicalFilesystemAccessor : IFilesystemAccessor
    {
        private readonly IActionLogger logger;

        public PhysicalFilesystemAccessor(IActionLogger logger)
        {
            this.logger = logger;
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
                this.logger.Log("You cannot move a file if you don't specify a target path.");
                return false;
            }

            if (!this.FileExists(sourceFilePath))
            {
                this.logger.Log("Before moving a file please make sure that file exists (Source: {0}, Target: {1}).", sourceFilePath, targetFilePath);
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
                this.logger.Log("Cannot move file \"{0}\" to \"{1}\". {2}", sourceFilePath, targetFilePath, exception);
            }

            return false;
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.logger.Log("You cannot delete a file if you don't specify a file name or path.");
                return false;
            }

            if (!this.FileExists(filePath))
            {
                this.logger.Log("The file you are trying to delete does not exist ({0}).", filePath);
                return false;
            }

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log("Cannot delete file \"{0}\" because it is being used. {1}", filePath, fileAccessException);
            }
            catch (Exception exception)
            {
                this.logger.Log("Cannot delete file \"{0}\". {1}", filePath, exception);
            }
            
            return false;
        }

        public bool DeleteFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                this.logger.Log("You cannot delete a folder if you don't specify a path.");
                return false;
            }

            if (!this.DirectoryExists(folderPath))
            {
                this.logger.Log("The folder you are trying to delete does not exist ({0}).", folderPath);
                return false;
            }

            try
            {
                Directory.Delete(folderPath, true);
                return true;
            }
            catch (IOException fileAccessException)
            {
                this.logger.Log("Cannot delete folder \"{0}\" because one or more files in it are being used. {1}", folderPath, fileAccessException);
            }
            catch (Exception exception)
            {
                this.logger.Log("Cannot delete folder \"{0}\". {1}", folderPath, exception);
            }

            return false;
        }
    }
}