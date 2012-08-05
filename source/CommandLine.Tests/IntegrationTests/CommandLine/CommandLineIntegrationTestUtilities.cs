using System.IO;

using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.CommandLine.Tests.IntegrationTests.CommandLine
{
    public static class CommandLineIntegrationTestUtilities
    {
        public static void RemoveAllFilesAndFoldersWhichAreCreatedOnStartup()
        {
            var appInfo = ApplicationInformationProvider.GetApplicationInformation();

            DeleteFolder(appInfo.LogFolder);
            DeleteFolder(appInfo.BuildFolder);
            DeleteFolder(appInfo.PrePackagingFolder);
            DeleteFolder(appInfo.PackagingFolder);

            DeleteFiles("NuDeploy.*.config", appInfo.ConfigurationFileFolder);
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