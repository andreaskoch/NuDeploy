using System;

namespace NuDeploy.Core.Common.Infrastructure
{
    public class ApplicationInformation
    {
        public string NameOfExecutable { get; set; }

        public Version ApplicationVersion { get; set; }

        public string ApplicationName { get; set; }

        public string StartupFolder { get; set; }

        public string ConfigurationFileFolder { get; set; }

        public string LogFolder { get; set; }

        public string BuildFolder { get; set; }

        public string PrePackagingFolder { get; set; }

        public string PackagingFolder { get; set; }

        public UserProperties ExecutingUser { get; set; }

        public string MachineName { get; set; }
    }
}