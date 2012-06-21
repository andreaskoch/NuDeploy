namespace NuDeploy.Core.Common
{
    public static class NuDeployConstants
    {
        public static readonly string ApplicationName = "NuDeploy";

        public static readonly string ExecutableName = string.Format("{0}.exe", ApplicationName);

        public static readonly string DefaultFeedUrl = "https://nuget.local-application-gallery.org/api/v2/";

        public static readonly string NuDeployCommandLinePackageId = string.Format("{0}.CommandLine", ApplicationName);
    }
}