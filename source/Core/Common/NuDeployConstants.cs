using System;

namespace NuDeploy.Core.Common
{
    public static class NuDeployConstants
    {
        public static readonly string ApplicationName = "NuDeploy";

        public static readonly string ExecutableName = string.Format("{0}.exe", ApplicationName);

        public static readonly Uri DefaultFeedUrl = new Uri("https://nuget.org/api/v2/");

        public static readonly string NuDeployCommandLinePackageId = string.Format("{0}.CommandLine", ApplicationName);

        public static readonly string CommonCommandOptionNameForce = "Force";

        public static readonly char MultiValueSeperator = ',';

        public const char NamespaceSeperator = '.';

        public const string PathSeperator = @"\";
    }
}