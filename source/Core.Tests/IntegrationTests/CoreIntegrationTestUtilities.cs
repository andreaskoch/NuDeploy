using System.IO;

using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Tests.IntegrationTests
{
    public static class CoreIntegrationTestUtilities
    {
        public static void RemoveAllFilesAndFoldersWhichAreCreatedOnStartup(ApplicationInformation applicationInformation)
        {
            DeleteFolder(applicationInformation.LogFolder);
            DeleteFolder(applicationInformation.BuildFolder);
            DeleteFolder(applicationInformation.PrePackagingFolder);
            DeleteFolder(applicationInformation.PackagingFolder);

            DeleteFiles("NuDeploy.*.config", applicationInformation.ConfigurationFileFolder);
        }

        public static void DeleteFiles(string filePattern, params string[] pathFragments)
        {
            string folder = Path.Combine(pathFragments);
            var files = Directory.GetFiles(folder, filePattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        } 
    }
}