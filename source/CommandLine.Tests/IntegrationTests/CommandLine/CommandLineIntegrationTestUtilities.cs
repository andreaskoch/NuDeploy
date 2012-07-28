using System.IO;

using NuDeploy.Core.Common.Infrastructure;

namespace CommandLine.Tests.IntegrationTests.CommandLine
{
    public static class CommandLineIntegrationTestUtilities
    {
        public static void Cleanup()
        {
            var appInfo = ApplicationInformationProvider.GetApplicationInformation();

            DeleteFolder(appInfo.LogFolder);
            DeleteFolder(appInfo.BuildFolder);
            DeleteFolder(appInfo.PrePackagingFolder);
            DeleteFolder(appInfo.PackagingFolder);
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