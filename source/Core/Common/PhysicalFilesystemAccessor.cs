using System.IO;

namespace NuDeploy.Core.Common
{
    public class PhysicalFilesystemAccessor : IFilesystemAccessor
    {
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
    }
}